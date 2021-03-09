using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.InMemory {
    public class InMemoryAdminLogRepository : InMemoryRepository<AdminLog, AdminLogId>, IAdminLogRepository {
        private readonly IUnitOfWork _unitOfWork;

        public InMemoryAdminLogRepository(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        protected override IList<AdminLog> GetBaseQuery() {
            return AuthUserDatabase.Get.AdminLogsTable.Select(u => u.ToDomainModel()).ToList();
        }

        public override void Save(AdminLog entity) {
            var dbAdminLog = DbAdminLog.FromDomainModel(entity);
            _unitOfWork.Store(dbAdminLog, tran => AuthUserDatabase.Temp(tran).AdminLogsTable);
        }
    }
}
