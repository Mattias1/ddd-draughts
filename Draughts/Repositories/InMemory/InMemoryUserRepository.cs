using Draughts.Domain.UserContext.Models;
using Draughts.Domain.UserContext.Specifications;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.InMemory;

public sealed class InMemoryUserRepository : InMemoryRepository<User, UserId>, IUserRepository {
    public InMemoryUserRepository(IRepositoryUnitOfWork unitOfWork) : base(unitOfWork) { }

    public User FindByName(string username) => Find(new UserUsernameSpecification(username));

    protected override IList<User> GetBaseQuery() {
        return UserDatabase.Get.UsersTable.Select(u => u.ToDomainModel()).ToList();
    }

    protected override void SaveInternal(User entity) {
        var user = DbUser.FromDomainModel(entity);
        UnitOfWork.Store(user, tran => UserDatabase.Temp(tran).UsersTable);
    }
}
