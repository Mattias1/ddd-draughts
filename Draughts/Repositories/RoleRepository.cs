using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.AuthUserAggregate.Specifications;
using Draughts.Repositories.Database;
using System.Collections.Generic;

namespace Draughts.Repositories {
    public class RoleRepository : Repository<Role>, IRoleRepository {
        protected override IList<Role> BaseQuery => AuthUserDatabase.RolesTable;

        public IReadOnlyList<Permission> PermissionsForRole(RoleId id) {
            var role = FindOrNull(new RoleIdSpecification(id));
            return role?.Permissions ?? new List<Permission>().AsReadOnly();
        }
    }
}
