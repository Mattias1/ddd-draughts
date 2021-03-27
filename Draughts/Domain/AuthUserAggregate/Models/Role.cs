using Draughts.Common;
using Draughts.Common.OoConcepts;
using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Domain.AuthUserAggregate.Models {
    public class Role : Entity<Role, RoleId> {
        public const string ADMIN_ROLENAME = "Admin";
        public const string PENDING_REGISTRATION_ROLENAME = "Pending registration";
        public const string REGISTERED_USER_ROLENAME = "Registered user";

        public override RoleId Id { get; }
        public string Rolename { get; private set; }
        public ZonedDateTime CreatedAt { get; }
        public IReadOnlyList<Permission> Permissions { get; private set; }

        public Role(RoleId id, string rolename, ZonedDateTime createdAt, params Permission[] permissions) {
            ValidateRolename(rolename);

            Id = id;
            Rolename = rolename;
            CreatedAt = createdAt;
            Permissions = permissions;
        }

        public IReadOnlyList<PermissionItem> BuildPermissionsList() {
            var enabledPermissions = Permissions.ToHashSet();
            return Permission.Permissions.All
                .Select(p => new PermissionItem { Permission = p, IsGranted = enabledPermissions.Contains(p) })
                .ToList()
                .AsReadOnly();
        }

        public void Edit(string rolename, IEnumerable<Permission> grantedPermissions) {
            ValidateRolename(rolename);
            DontLockYourselfOut(rolename, grantedPermissions);

            Rolename = rolename;
            Permissions = grantedPermissions.ToList().AsReadOnly();
        }

        private void ValidateRolename(string rolename) {
            if (rolename.Length < 3) {
                throw new ManualValidationException("Invalid rolename.");
            }
        }

        private void DontLockYourselfOut(string newName, IEnumerable<Permission> newPermissions) {
            if (Rolename != Role.ADMIN_ROLENAME) {
                return;
            }

            if (newName != Role.ADMIN_ROLENAME) {
                throw new ManualValidationException("You are not allowed to change the admin role's name.");
            }
            if (Permissions.Except(Permission.Permissions.IgnoredByAdmins).Except(newPermissions).Any()) {
                throw new ManualValidationException("You are not allowed to remove permissions from the admin role.");
            }
        }

        public readonly struct PermissionItem {
            public Permission Permission { get; init; }
            public bool IsGranted { get; init; }
        }
    }
}
