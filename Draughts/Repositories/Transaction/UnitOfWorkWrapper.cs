using System;

namespace Draughts.Repositories.Transaction;

public sealed class UnitOfWorkWrapper : IUnitOfWork {
    private readonly IRepositoryUnitOfWork _realUnitOfWork;

    public UnitOfWorkWrapper(IRepositoryUnitOfWork realUnitOfWork) {
        _realUnitOfWork = realUnitOfWork;
    }

    public void WithAuthTransaction(Action<ITransaction> function) => _realUnitOfWork.WithAuthTransaction(function);
    public void WithGameTransaction(Action<ITransaction> function) => _realUnitOfWork.WithGameTransaction(function);
    public void WithUserTransaction(Action<ITransaction> function) => _realUnitOfWork.WithUserTransaction(function);
    public T WithAuthTransaction<T>(Func<ITransaction, T> function) => _realUnitOfWork.WithAuthTransaction(function);
    public T WithGameTransaction<T>(Func<ITransaction, T> function) => _realUnitOfWork.WithGameTransaction(function);
    public T WithUserTransaction<T>(Func<ITransaction, T> function) => _realUnitOfWork.WithUserTransaction(function);

    public void WithTransaction(TransactionDomain domain, Action<ITransaction> function)
        => _realUnitOfWork.WithTransaction(domain, function);
    public T WithTransaction<T>(TransactionDomain domain, Func<ITransaction, T> function)
        => _realUnitOfWork.WithTransaction(domain, function);

    public ITransaction BeginTransaction(TransactionDomain domain) => _realUnitOfWork.BeginTransaction(domain);
}
