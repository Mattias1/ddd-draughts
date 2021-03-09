using Draughts.Common.Events;
using NodaTime;
using SqlQueryBuilder.Builder;
using System;
using System.Collections.Generic;

namespace Draughts.Repositories.Transaction {
    public interface IUnitOfWork {
        void WithAuthUserTransaction(Action<ITransaction> function);
        void WithGameTransaction(Action<ITransaction> function);
        void WithUserTransaction(Action<ITransaction> function);
        T WithAuthUserTransaction<T>(Func<ITransaction, T> function);
        T WithGameTransaction<T>(Func<ITransaction, T> function);
        T WithUserTransaction<T>(Func<ITransaction, T> function);

        void WithTransaction(TransactionDomain domain, Action<ITransaction> function);
        T WithTransaction<T>(TransactionDomain domain, Func<ITransaction, T> function);
        ITransaction BeginTransaction(TransactionDomain domain);

        void Register(IDomainEventHandler eventHandler);
        void Raise(Func<DomainEventId, ZonedDateTime, DomainEvent> evtFunc);
        void Raise(DomainEvent evt);
        void FireAll();

        void Store<T>(T obj, Func<ITransaction, List<T>> tableFunc) where T : IEquatable<T>;

        IInitialQueryBuilder Query(TransactionDomain domain);
    }
}
