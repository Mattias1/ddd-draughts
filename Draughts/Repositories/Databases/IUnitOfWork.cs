using Draughts.Common.Events;
using System;
using System.Collections.Generic;

namespace Draughts.Repositories.Databases {
    public interface IUnitOfWork {
        InMemoryUnitOfWork.Transaction BeginTransaction(TransactionDomain domain);
        void FireAll();
        void Raise(DomainEvent evt);
        void Register(IDomainEventHandler eventHandler);
        void Store<T>(T obj, List<T> table) where T : IEquatable<T>;
        void WithTransaction(TransactionDomain domain, Action<InMemoryUnitOfWork.Transaction> function);
        T WithTransaction<T>(TransactionDomain domain, Func<InMemoryUnitOfWork.Transaction, T> function);
    }
}