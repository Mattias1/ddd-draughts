using System.Collections.Generic;

namespace Draughts.Domain.AuthUserAggregate.Models {
    public class Role {
        public const string ADMIN_ROLENAME = "Admin";
        public const string PENDING_REGISTRATION_ROLENAME = "Pending registration";
        public const string REGISTERED_USER_ROLENAME = "Registered user";

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
