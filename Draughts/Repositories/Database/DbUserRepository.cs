using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories.Transaction;
using SqlQueryBuilder.Builder;

namespace Draughts.Repositories.Database {
    public class DbUserRepository : DbRepository<User, UserId, DbUser>, IUserRepository {
        private readonly IUnitOfWork _unitOfWork;

        public DbUserRepository(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        protected override string TableName => "user";
        protected override IInitialQueryBuilder GetBaseQuery() => _unitOfWork.Query(TransactionDomain.User);

        protected override User Parse(DbUser q) => q.ToDomainModel();

        public override void Save(User entity) {
            var obj = DbUser.FromDomainModel(entity);
            if (FindByIdOrNull(entity.Id) is null) {
                GetBaseQuery().InsertInto(TableName).InsertFrom(obj).Execute();
            }
            else {
                GetBaseQuery().Update(TableName).SetWithoutIdFrom(obj).Where("id").Is(entity.Id).Execute();
            }
        }
    }
}
