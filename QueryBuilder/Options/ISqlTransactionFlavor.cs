using System;
using System.Threading.Tasks;

namespace SqlQueryBuilder.Options;

public interface ISqlTransactionFlavor : ISqlFlavor, IDisposable {
    Task CommitAsync();
    void Commit();
    Task RollbackAsync();
    void Rollback();
}
