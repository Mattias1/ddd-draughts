using Draughts.Common.OoConcepts;
using NodaTime;
using System.Collections.Generic;

namespace Draughts.Domain.AuthUserAggregate.Models {
    public class Role : Entity<Role, RoleId> {
        public const string ADMIN_ROLENAME = "Admin";
        public const string PENDING_REGISTRATION_ROLENAME = "Pending registration";
        public const string REGISTERED_USER_ROLENAME = "Registered user";

        public override RoleId Id { get; }
        public string Rolename { get; }
        public ZonedDateTime CreatedAt { get; }
        public IReadOnlyList<Permission> Permissions { get; }

        public Role(RoleId id, string rolename, ZonedDateTime createdAt, params Permission[] permissions) {
            Id = id;
            Rolename = rolename;
            CreatedAt = createdAt;
            Permissions = permissions;
        }
    }
}
