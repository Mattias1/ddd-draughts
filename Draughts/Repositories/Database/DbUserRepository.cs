using Draughts.Domain.UserContext.Models;
using Draughts.Domain.UserContext.Specifications;
using Draughts.Repositories.Transaction;
using SqlQueryBuilder.Builder;

namespace Draughts.Repositories.Database;

public sealed class DbUserRepository : DbRepository<User, UserId, DbUser>, IUserRepository {
    public DbUserRepository(IRepositoryUnitOfWork unitOfWork) : base(unitOfWork) { }

    public User FindByName(string username) => Find(new UserUsernameSpecification(username));

    protected override string TableName => "user";
    protected override IInitialQueryBuilder GetBaseQuery() => UnitOfWork.Query(TransactionDomain.User);

    protected override User Parse(DbUser q) => q.ToDomainModel();

    protected override void SaveInternal(User entity) {
        var obj = DbUser.FromDomainModel(entity);
        if (FindByIdOrNull(entity.Id) is null) {
            GetBaseQuery().InsertInto(TableName).InsertFrom(obj).Execute();
        }
        else {
            GetBaseQuery().Update(TableName).SetWithoutIdFrom(obj).Where("id").Is(entity.Id).Execute();
        }
    }
}
