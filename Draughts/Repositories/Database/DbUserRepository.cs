using Draughts.Domain.AuthUserAggregate.Specifications;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories.Transaction;
using SqlQueryBuilder.Builder;

namespace Draughts.Repositories.Database {
    public class DbUserRepository : DbRepository<User, DbUser>, IUserRepository {
        private readonly IUnitOfWork _unitOfWork;

        public DbUserRepository(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public User FindById(UserId id) => Find(new UserIdSpecification(id));
        public User? FindByIdOrNull(UserId id) => FindOrNull(new UserIdSpecification(id));

        protected override string TableName => "user";
        protected override IInitialQueryBuilder GetBaseQuery() => _unitOfWork.Query(TransactionDomain.User);

        protected override User Parse(DbUser q) => q.ToDomainModel();

        public override void Save(User entity) {
            var obj = DbUser.FromDomainModel(entity);
            if (FindByIdOrNull(entity.Id) is null) {
                GetBaseQuery().InsertInto("user").InsertFrom(obj).Execute();
            }
            else {
                GetBaseQuery().Update("user").SetWithoutIdFrom(obj).Where("id").Is(entity.Id).Execute();
            }
        }
    }
}
