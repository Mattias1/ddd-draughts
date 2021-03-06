using Draughts.Domain.AuthUserContext.Models;
using Draughts.Repositories.Transaction;
using SqlQueryBuilder.Builder;
using System;

namespace Draughts.Repositories.Database {
    public class DbAdminLogRepository : DbRepository<AdminLog, AdminLogId, DbAdminLog>, IAdminLogRepository {
        private readonly IUnitOfWork _unitOfWork;

        public DbAdminLogRepository(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        protected override string TableName => "adminlog";
        protected override IInitialQueryBuilder GetBaseQuery() => _unitOfWork.Query(TransactionDomain.AuthUser);

        protected override AdminLog Parse(DbAdminLog q) => q.ToDomainModel();

        public override void Save(AdminLog entity) {
            var obj = DbAdminLog.FromDomainModel(entity);
            if (FindByIdOrNull(entity.Id) is null) {
                GetBaseQuery().InsertInto(TableName).InsertFrom(obj).Execute();
            }
            else {
                throw new InvalidOperationException("You cannot edit an audit log entry.");
            }
        }
    }
}
