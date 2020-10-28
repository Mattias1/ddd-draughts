using Draughts.Common.Events;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Draughts.Repositories.Databases {
    public class InMemoryUnitOfWork : IUnitOfWork {
        private readonly AsyncLocal<TransactionDomain?> _currentTransactionDomain;
        private readonly List<IDomainEventHandler> _eventHandlers;
        private readonly EventQueue _eventQueue;
        private readonly Dictionary<string, Transaction> _openTransactions;

        private readonly object _lock = new object();

        private TransactionDomain CurrentTransactionDomain {
            get => _currentTransactionDomain.Value ?? throw new InvalidOperationException("There is no open transaction.");
        }

        public InMemoryUnitOfWork(IClock clock) {
            _currentTransactionDomain = new AsyncLocal<TransactionDomain?>();
            _eventHandlers = new List<IDomainEventHandler>();
            _eventQueue = new EventQueue(clock, _eventHandlers);
            _openTransactions = new Dictionary<string, Transaction>();
        }

        public void WithTransaction(TransactionDomain domain, Action<Transaction> function) {
            using (var transaction = BeginTransaction(domain)) {
                function(transaction);
            }
        }
        public T WithTransaction<T>(TransactionDomain domain, Func<Transaction, T> function) {
            using (var transaction = BeginTransaction(domain)) {
                return function(transaction);
            }
        }

        public Transaction BeginTransaction(TransactionDomain domain) {
            lock (_lock) {
                if (_openTransactions.ContainsKey(domain.Key)) {
                    string open = _openTransactions[domain.Key].IsOpen ? "open" : "closed";
                    throw new InvalidOperationException($"Errr, this transaction is already started ({open}).");
                }

                _currentTransactionDomain.Value = domain;

                var transaction = new Transaction(domain);
                transaction.OnOpened += (o, e) => OnOpenedTransaction(o, e);
                transaction.OnClosed += (o, e) => OnClosedTransaction(o, e);
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

        public void Store<T>(T obj, List<T> table) where T : IEquatable<T> {
            CurrentTransactionDomain.InMemoryStore(obj, table);
        }

        public void Register(IDomainEventHandler eventHandler) => _eventHandlers.Add(eventHandler);

        public void Raise(DomainEvent evt) {
            var domain = CurrentTransactionDomain;
            domain.InMemoryStore(evt, domain.TempDomainEventsTable);
        }

        public void FireAll() => _eventQueue.FireAll();

        public class Transaction : IDisposable {
            private List<DomainEvent> _flushedEvents;
            private readonly TransactionDomain _transactionDomain;

            public bool IsOpen { get; private set; }
            public bool Succeeded { get; private set; }
            public IReadOnlyList<DomainEvent> RaisedEvents => _flushedEvents.AsReadOnly();

            public delegate void TransactionEventHandler(Transaction transaction, TransactionEventArgs e);

            /// <summary>
            /// Called after the transaction is opened
            /// </summary>
            public event TransactionEventHandler? OnOpened;
            /// <summary>
            /// Called after the transaction is closed
            /// </summary>
            public event TransactionEventHandler? OnClosed;

            public Transaction(TransactionDomain domain) {
                _flushedEvents = new List<DomainEvent>();
                _transactionDomain = domain;
                IsOpen = false;
            }

            public void Start() {
                IsOpen = true;
                OnOpened?.Invoke(this, new TransactionEventArgs(_transactionDomain));
            }

            public void Flush() {
                _flushedEvents.AddRange(_transactionDomain.TempDomainEventsTable);

                _transactionDomain.InMemoryFlush();
            }

            public void Commit() {
                if (!IsOpen) {
                    throw new InvalidOperationException("Cannot commit the transaction because it isn't open.");
                }

                Flush();
                IsOpen = false;
                Succeeded = true;
                OnClosed?.Invoke(this, new TransactionEventArgs(_transactionDomain));
            }

            public void Rollback() {
                if (!IsOpen) {
                    throw new InvalidOperationException("Cannot rollback the transaction because it isn't open.");
                }

                _transactionDomain.InMemoryRollback();
                IsOpen = false;
                OnClosed?.Invoke(this, new TransactionEventArgs(_transactionDomain));
            }

            public void Dispose() {
                if (IsOpen) {
                    Rollback();
                }
            }
        }

        public class TransactionEventArgs : EventArgs {
            public TransactionDomain TransactionDomain { get; }
            public TransactionEventArgs(TransactionDomain domain) => TransactionDomain = domain;
        }
    }
}
