using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.AuthUserAggregate.Specifications;
using Draughts.Repositories.Database;
using System.Collections.Generic;

namespace Draughts.Repositories {
    public class AuthUserRepository : Repository<AuthUser>, IAuthUserRepository {
        protected override IList<AuthUser> BaseQuery => AuthUserDatabase.AuthUsersTable;

        public AuthUser FindById(AuthUserId id) => Find(new AuthUserIdSpecification(id));
        public AuthUser? FindByIdOrNull(AuthUserId id) => FindOrNull(new AuthUserIdSpecification(id));
    }
}
