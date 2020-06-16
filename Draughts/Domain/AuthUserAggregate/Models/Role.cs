using System.Collections.Generic;

namespace Draughts.Domain.AuthUserAggregate.Models {
    public class Role {
        public RoleId Id { get; }
        public string Rolename { get; }
        public IReadOnlyList<Permission> Permissions { get; }

        public Role(RoleId id, string rolename, params Permission[] permissions) {
            Id = id;
            Rolename = rolename;
            Permissions = permissions;
        }
    }
}
