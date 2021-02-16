using Draughts.Common.Events;
using Draughts.Repositories.Database;
using NodaTime;
using SqlQueryBuilder.Builder;
using SqlQueryBuilder.Options;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Draughts.Repositories.Transaction {
    public class DbUnitOfWork : IUnitOfWork {
        private readonly AsyncLocal<Transaction?> _currentTransaction;
        private readonly List<IDomainEventHandler> _eventHandlers;
        private readonly EventQueue _eventQueue;

        private readonly object _lock = new object();

        public DbUnitOfWork(IClock clock) {
            _currentTransaction = new AsyncLocal<Transaction?>();
            _eventHandlers = new List<IDomainEventHandler>();
            _eventQueue = new EventQueue(clock, _eventHandlers);
        }

        public void WithAuthUserTransaction(Action<ITransaction> function) => WithTransaction(TransactionDomain.AuthUser, function);
        public void WithGameTransaction(Action<ITransaction> function) => WithTransaction(TransactionDomain.Game, function);
        public void WithUserTransaction(Action<ITransaction> function) => WithTransaction(TransactionDomain.User, function);
        public T WithAuthUserTransaction<T>(Func<ITransaction, T> function) => WithTransaction(TransactionDomain.AuthUser, function);
        public T WithGameTransaction<T>(Func<ITransaction, T> function) => WithTransaction(TransactionDomain.Game, function);
        public T WithUserTransaction<T>(Func<ITransaction, T> function) => WithTransaction(TransactionDomain.User, function);


        public void WithTransaction(TransactionDomain domain, Action<ITransaction> function) {
            using (var transaction = BeginTransaction(domain)) {
                function(transaction);
            }
        }
        public T WithTransaction<T>(TransactionDomain domain, Func<ITransaction, T> function) {
            using (var transaction = BeginTransaction(domain)) {
                return function(transaction);
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
                _eventQueue.Enqueue(transaction.RaisedEvents);
                _eventQueue.FireAll();
            }
        }

        public void Register(IDomainEventHandler eventHandler) => _eventHandlers.Add(eventHandler);

        public void Raise(DomainEvent evt) {
            if (_currentTransaction.Value is null) {
                throw new InvalidOperationException("You can only raise events from within a transaction context.");
            }

            _currentTransaction.Value.RaiseEvent(evt);
        }

        public void FireAll() => _eventQueue.FireAll();

        public void Store<T>(T obj, List<T> table) where T : IEquatable<T> {
            throw new InvalidOperationException("Store through the repositories, not through the unit of work.");
        }

        public IInitialQueryBuilder Query(TransactionDomain domain) {
            if (_currentTransaction.Value is null) {
                throw new InvalidOperationException("You can only start a query from withing a transaction context.");
            }
            return _currentTransaction.Value.Query(domain);
        }

        public class Transaction : ITransaction {
            private List<DomainEvent> _raisedEvents;
            private readonly TransactionDomain _transactionDomain;
            private ISqlTransactionFlavor? _transactionFlavor;

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
                // TODO: Query that stores the event
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

            public T CommitWith<T>(T result) {
                Commit();
                return result;
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
}
