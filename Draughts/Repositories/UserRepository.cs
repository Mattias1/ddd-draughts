using Draughts.Domain.AuthUserAggregate.Specifications;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories.Database;
using System.Collections.Generic;

namespace Draughts.Repositories {
    public class UserRepository : Repository<User>, IUserRepository {
        protected override IList<User> BaseQuery => UserDatabase.UsersTable;

        public User FindById(UserId id) => Find(new UserIdSpecification(id));
        public User? FindByIdOrNull(UserId id) => FindOrNull(new UserIdSpecification(id));
    }
}
