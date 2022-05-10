using Draughts.Common.Events;
using Draughts.Common.Utilities;
using NodaTime;
using SqlQueryBuilder.Builder;
using System;
using System.Collections.Generic;
using System.Threading;
using static Draughts.Common.Events.DomainEvent;
using static Draughts.Repositories.Transaction.PairTableFunctions;
using static Draughts.Repositories.Transaction.TransactionDomain;

namespace Draughts.Repositories.Transaction;

public sealed class InMemoryUnitOfWork : IRepositoryUnitOfWork {
    private readonly IClock _clock;
    private readonly IIdGenerator _idGenerator;

    private readonly AsyncLocal<TransactionDomain?> _currentTransactionDomain;
    private readonly List<IDomainEventHandler> _eventHandlers;
    private readonly EventQueue _eventQueue;
    private readonly Dictionary<string, Transaction> _openTransactions;

    private static readonly object _lock = new object();

    private TransactionDomain CurrentTransactionDomain {
        get => _currentTransactionDomain.Value ?? throw new InvalidOperationException("There is no open transaction.");
    }

    public InMemoryUnitOfWork(IClock clock, IIdGenerator idGenerator) {
        _clock = clock;
        _idGenerator = idGenerator;

        _currentTransactionDomain = new AsyncLocal<TransactionDomain?>();
        _eventHandlers = new List<IDomainEventHandler>();
        _eventQueue = new EventQueue(clock, _eventHandlers);
        _openTransactions = new Dictionary<string, Transaction>();
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
            if (_openTransactions.ContainsKey(domain.Key)) {
                string open = _openTransactions[domain.Key].IsOpen ? "open" : "closed";
                throw new InvalidOperationException($"Errr, this transaction is already started ({open}).");
            }

            if (_currentTransactionDomain.Value is not null) {
                throw new InvalidOperationException("You already have a transaction within this thread.");
            }

            _currentTransactionDomain.Value = domain;

            var transaction = new Transaction(domain);
            transaction.OnOpened += (o, e) => OnOpenedTransaction((Transaction)o, e);
            transaction.OnClosed += (o, e) => OnClosedTransaction((Transaction)o, e);
            transaction.Start();

            return transaction;
        }
    }

    private void OnOpenedTransaction(Transaction transaction, TransactionEventArgs e) {
        _openTransactions.Add(e.TransactionDomain.Key, transaction);
    }

    private void OnClosedTransaction(Transaction transaction, TransactionEventArgs e) {
        lock (_lock) {
            _currentTransactionDomain.Value = null;
            _openTransactions.Remove(e.TransactionDomain.Key);
        }

        if (transaction.Succeeded) {
            _eventQueue.Enqueue(transaction.RaisedEvents);
            _eventQueue.DispatchAll();
        }
    }

    public void Register(IDomainEventHandler eventHandler) => _eventHandlers.Add(eventHandler);

    public void Raise(DomainEventFactory eventFactory) {
        var nextId = new DomainEventId(_idGenerator.ReservePool(1, 0, 0).Next());
        Raise(eventFactory(nextId, _clock.UtcNow()));
    }
    public void Raise(DomainEvent evt) {
        Store(evt, tran => CurrentTransactionDomain.TempDomainEventsTable(tran));
    }

    public void DispatchAll() => _eventQueue.DispatchAll();

    public void Store<T>(T obj, Func<ITransaction, List<T>> tableFunc) where T : IEquatable<T> {
        var tran = _openTransactions[CurrentTransactionDomain.Key];
        InMemoryDatabaseUtils.StoreInto(obj, tableFunc(tran));
    }

    public IInitialQueryBuilder Query(TransactionDomain domain) {
        throw new InvalidOperationException("Use the Store method, not queries.");
    }

    public sealed class Transaction : ITransaction {
        private List<DomainEvent> _committedEvents;
        private readonly TransactionDomain _transactionDomain;

        public bool IsOpen { get; private set; }
        public bool Succeeded { get; private set; }
        public IReadOnlyList<DomainEvent> RaisedEvents => _committedEvents.AsReadOnly();

        /// <summary>
        /// Called after the transaction is opened
        /// </summary>
        public event TransactionEventHandler? OnOpened;
        /// <summary>
        /// Called after the transaction is closed
        /// </summary>
        public event TransactionEventHandler? OnClosed;

        public Transaction(TransactionDomain domain) {
            _committedEvents = new List<DomainEvent>();
            _transactionDomain = domain;
            IsOpen = false;
        }

        public void Start() {
            if (IsOpen) {
                throw new InvalidOperationException("Cannot start a transaction twice.");
            }

            _transactionDomain.CreateTempDatabase(this);

            IsOpen = true;
            Succeeded = false;
            OnOpened?.Invoke(this, new TransactionEventArgs(_transactionDomain));
        }

        public void Commit() {
            if (!IsOpen) {
                throw new InvalidOperationException("Cannot commit the transaction because it isn't open.");
            }

            _committedEvents.AddRange(_transactionDomain.TempDomainEventsTable(this));
            _transactionDomain.ApplyForAllTablePairs(this, new StoreIntoFunction());
            _transactionDomain.ApplyForAllTablePairs(this, new ClearTempFunction());

            IsOpen = false;
            Succeeded = true;
        }

        public void Rollback() {
            if (!IsOpen) {
                throw new InvalidOperationException("Cannot rollback the transaction because it isn't open.");
            }

            _transactionDomain.ApplyForAllTablePairs(this, new ClearTempFunction());
            _transactionDomain.RemoveTempDatabase(this);

            IsOpen = false;
        }

        public void Dispose() {
            if (IsOpen) {
                Rollback();
            }
            OnClosed?.Invoke(this, new TransactionEventArgs(_transactionDomain));
        }
    }
}
