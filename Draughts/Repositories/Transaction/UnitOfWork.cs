using Draughts.Common.Events;
using Draughts.Common.Utilities;
using Draughts.Repositories.Misc;
using NodaTime;
using SqlQueryBuilder.Builder;
using SqlQueryBuilder.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using static Draughts.Common.Events.DomainEvent;

namespace Draughts.Repositories.Transaction;

public sealed class UnitOfWork : IRepositoryUnitOfWork {
    private readonly IClock _clock;
    private readonly IIdGenerator _idGenerator;

    private readonly AsyncLocal<Transaction?> _currentTransaction;
    private readonly EventDispatcher _eventDispatcher;

    private static readonly object _lock = new object();

    public UnitOfWork(IClock clock, EventDispatcher eventDispatcher, IIdGenerator idGenerator) {
        _clock = clock;
        _eventDispatcher = eventDispatcher;
        _idGenerator = idGenerator;

        _currentTransaction = new AsyncLocal<Transaction?>();
    }

    public TransactionDomain ActiveTransactionDomain() {
        return _currentTransaction?.Value?.TransactionDomain
                ?? throw new InvalidOperationException("There is no open transaction.");
    }

    public void WithAuthTransaction(Action<ITransaction> function) => WithTransaction(TransactionDomain.Auth, function);
    public void WithGameTransaction(Action<ITransaction> function) => WithTransaction(TransactionDomain.Game, function);
    public void WithUserTransaction(Action<ITransaction> function) => WithTransaction(TransactionDomain.User, function);
    public T WithAuthTransaction<T>(Func<ITransaction, T> function) => WithTransaction(TransactionDomain.Auth, function);
    public T WithGameTransaction<T>(Func<ITransaction, T> function) => WithTransaction(TransactionDomain.Game, function);
    public T WithUserTransaction<T>(Func<ITransaction, T> function) => WithTransaction(TransactionDomain.User, function);

    public void WithTransaction(TransactionDomain domain, Action<ITransaction> function) {
        using (var transaction = BeginTransaction(domain)) {
            function(transaction);
            if (transaction.IsOpen) {
                transaction.Commit();
            }
        }
    }
    public T WithTransaction<T>(TransactionDomain domain, Func<ITransaction, T> function) {
        using (var transaction = BeginTransaction(domain)) {
            var result = function(transaction);
            if (transaction.IsOpen) {
                transaction.Commit();
            }
            return result;
        }
    }

    public ITransaction BeginTransaction(TransactionDomain domain) {
        lock (_lock) {
            if (_currentTransaction.Value is not null) {
                throw new InvalidOperationException("You already have a transaction within this thread.");
            }

            var transaction = new Transaction(domain);
            transaction.OnClosed += (o, e) => OnClosedTransaction((Transaction)o, e);
            transaction.Start();

            _currentTransaction.Value = transaction;

            return transaction;
        }
    }

    private void OnClosedTransaction(Transaction transaction, TransactionEventArgs e) {
        lock (_lock) {
            _currentTransaction.Value = null;
        }

        if (transaction.Succeeded) {
            _eventDispatcher.DispatchAll(transaction.RaisedEvents);
        }
    }

    public DomainEvent Raise(DomainEventFactory eventFactory) {
        var nextId = new DomainEventId(_idGenerator.ReservePool(1, 0, 0).Next());
        return Raise(eventFactory(nextId, _clock.UtcNow()));
    }
    public DomainEvent Raise(DomainEvent evt) {
        if (_currentTransaction.Value is null) {
            throw new InvalidOperationException("You can only raise events from within a transaction context.");
        }

        _currentTransaction.Value.RaiseEvent(evt);
        return evt;
    }

    public IInitialQueryBuilder Query(TransactionDomain domain) {
        if (_currentTransaction.Value is null) {
            throw new InvalidOperationException("You can only start a query from withing a transaction context.");
        }
        return _currentTransaction.Value.Query(domain);
    }

    public sealed class Transaction : ITransaction {
        private List<DomainEvent> _raisedEvents;
        private readonly TransactionDomain _transactionDomain;
        private ISqlTransactionFlavor? _transactionFlavor;

        public TransactionDomain TransactionDomain => _transactionDomain;
        public bool IsOpen { get; private set; }
        public bool Succeeded { get; private set; }
        public IReadOnlyList<DomainEvent> RaisedEvents => _raisedEvents.AsReadOnly();

        /// <summary>
        /// Called after the transaction is opened
        /// </summary>
        public event TransactionEventHandler? OnOpened;
        /// <summary>
        /// Called after the transaction is closed
        /// </summary>
        public event TransactionEventHandler? OnClosed;

        public Transaction(TransactionDomain domain) {
            _raisedEvents = new List<DomainEvent>();
            _transactionDomain = domain;
            IsOpen = false;
        }

        public void RaiseEvent(DomainEvent evt) {
            _raisedEvents.Add(evt);
        }

        public void Start() {
            if (IsOpen) {
                throw new InvalidOperationException("Cannot start a transaction twice.");
            }

            _raisedEvents.Clear();
            _transactionFlavor = _transactionDomain.BeginTransaction();

            IsOpen = true;
            Succeeded = false;
            OnOpened?.Invoke(this, new TransactionEventArgs(_transactionDomain));
        }

        public void Commit() {
            if (!IsOpen) {
                throw new InvalidOperationException("Cannot commit the transaction because it isn't open.");
            }

            _transactionFlavor!.Commit();

            IsOpen = false;
            Succeeded = true;
        }

        public void Rollback() {
            if (!IsOpen) {
                throw new InvalidOperationException("Cannot rollback the transaction because it isn't open.");
            }

            _raisedEvents.Clear();
            _transactionFlavor!.Rollback();

            IsOpen = false;
        }

        public void Dispose() {
            if (IsOpen) {
                Rollback();
            }
            _transactionFlavor?.Dispose();
            OnClosed?.Invoke(this, new TransactionEventArgs(_transactionDomain));
        }

        public IInitialQueryBuilder Query(TransactionDomain domain) {
            if (_transactionDomain != domain) {
                throw new InvalidOperationException("The transaction does not have the specified domain.");
            }
            return Query();
        }
        public IInitialQueryBuilder Query() {
            if (_transactionFlavor is null || !IsOpen) {
                throw new InvalidOperationException("Cannot start a query unless the transaction is open.");
            }
            return DbContext.Get.Query(_transactionFlavor);
        }
    }
}
