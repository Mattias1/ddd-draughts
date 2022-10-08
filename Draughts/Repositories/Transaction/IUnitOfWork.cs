using Draughts.Common.Events;
using SqlQueryBuilder.Builder;
using System;
using static Draughts.Common.Events.DomainEvent;

namespace Draughts.Repositories.Transaction;

// This unit of work is only meant to be used by repositories - the rest of the code shouldn't
// concern itself with manual queries or raising events.
public interface IRepositoryUnitOfWork : IUnitOfWork {
    DomainEvent Raise(DomainEventFactory eventFactory);
    DomainEvent Raise(DomainEvent evt);

    TransactionDomain ActiveTransactionDomain();

    IInitialQueryBuilder Query(TransactionDomain domain);
}

// This unit of work can be used by anyone.
public interface IUnitOfWork {
    void WithAuthTransaction(Action<ITransaction> function);
    void WithGameTransaction(Action<ITransaction> function);
    void WithUserTransaction(Action<ITransaction> function);
    T WithAuthTransaction<T>(Func<ITransaction, T> function);
    T WithGameTransaction<T>(Func<ITransaction, T> function);
    T WithUserTransaction<T>(Func<ITransaction, T> function);

    void WithTransaction(TransactionDomain domain, Action<ITransaction> function);
    T WithTransaction<T>(TransactionDomain domain, Func<ITransaction, T> function);
    ITransaction BeginTransaction(TransactionDomain domain);
}
