using Draughts.Common.Events;
using Draughts.Common.Utilities;
using NodaTime;
using SqlQueryBuilder.Builder;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Draughts.Repositories.Transaction {
    public class InMemoryUnitOfWork : IUnitOfWork {
        private readonly IClock _clock;
        private readonly IIdGenerator _idGenerator;

        private readonly AsyncLocal<TransactionDomain?> _currentTransactionDomain;
        private readonly List<IDomainEventHandler> _eventHandlers;
        private readonly EventQueue _eventQueue;
        private readonly Dictionary<string, Transaction> _openTransactions;

        private readonly object _lock = new object();

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
                _eventQueue.FireAll();
            }
        }

        public void Register(IDomainEventHandler eventHandler) => _eventHandlers.Add(eventHandler);

        public void Raise(Func<DomainEventId, ZonedDateTime, DomainEvent> evtFunc) {
            var nextId = new DomainEventId(_idGenerator.ReservePool(1, 0, 0).Next());
            Raise(evtFunc(nextId, _clock.UtcNow()));
        }
        public void Raise(DomainEvent evt) {
            var domain = CurrentTransactionDomain;
            domain.InMemoryStore(evt, domain.TempDomainEventsTable);
        }

        public void FireAll() => _eventQueue.FireAll();

        public void Store<T>(T obj, List<T> table) where T : IEquatable<T> {
            CurrentTransactionDomain.InMemoryStore(obj, table);
        }

        public IInitialQueryBuilder Query(TransactionDomain domain) {
            throw new InvalidOperationException("Use the Store method, not queries.");
        }

        public class Transaction : ITransaction {
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

                _committedEvents.AddRange(_transactionDomain.TempDomainEventsTable);
                _transactionDomain.InMemoryFlush();

                IsOpen = false;
                Succeeded = true;
            }

            public void Rollback() {
                if (!IsOpen) {
                    throw new InvalidOperationException("Cannot rollback the transaction because it isn't open.");
                }

                _transactionDomain.InMemoryRollback();

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
}
