using Draughts.Domain.AuthContext.Models;
using Draughts.Repositories.Transaction;
using SqlQueryBuilder.Builder;
using System;

namespace Draughts.Repositories.Database;

public sealed class DbAdminLogRepository : DbRepository<AdminLog, AdminLogId, DbAdminLog>, IAdminLogRepository {
    public DbAdminLogRepository(IRepositoryUnitOfWork unitOfWork) : base (unitOfWork) { }

    protected override string TableName => "adminlog";
    protected override IInitialQueryBuilder GetBaseQuery() => UnitOfWork.Query(TransactionDomain.Auth);

    protected override AdminLog Parse(DbAdminLog q) => q.ToDomainModel();

    protected override void SaveInternal(AdminLog entity) {
        var obj = DbAdminLog.FromDomainModel(entity);
        if (FindByIdOrNull(entity.Id) is null) {
            GetBaseQuery().InsertInto(TableName).InsertFrom(obj).Execute();
        }
        else {
            throw new InvalidOperationException("You cannot edit an audit log entry.");
        }
    }
}
