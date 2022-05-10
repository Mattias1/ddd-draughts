using Draughts.Domain.AuthContext.Models;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.InMemory;

public sealed class InMemoryAdminLogRepository : InMemoryRepository<AdminLog, AdminLogId>, IAdminLogRepository {
    public InMemoryAdminLogRepository(IRepositoryUnitOfWork unitOfWork) : base(unitOfWork) { }

    protected override IList<AdminLog> GetBaseQuery() {
        return AuthDatabase.Get.AdminLogsTable.Select(u => u.ToDomainModel()).ToList();
    }

    protected override void SaveInternal(AdminLog entity) {
        var dbAdminLog = DbAdminLog.FromDomainModel(entity);
        UnitOfWork.Store(dbAdminLog, tran => AuthDatabase.Temp(tran).AdminLogsTable);
    }
}
