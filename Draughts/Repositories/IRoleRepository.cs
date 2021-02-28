using Draughts.Domain.AuthUserAggregate.Models;
using System.Collections.Generic;

namespace Draughts.Repositories {
    public interface IRoleRepository : IRepository<Role, RoleId> {
        IReadOnlyList<Permission> PermissionsForRole(RoleId roleId);
    }
}
