using Draughts.Common;
using Draughts.Common.OoConcepts;
using Draughts.Domain.UserAggregate.Models;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Domain.AuthUserAggregate.Models {
    public class AuthUser : Entity<AuthUser, UserId> {
        private static IReadOnlyList<string> PROTECTED_USERS => new [] { Username.ADMIN, Username.MATTY };

        private readonly List<Role> _roles; // TODO: These should be RoleIds, not Roles.

        public override UserId Id { get; }
        public Username Username { get; private set; }
        public PasswordHash PasswordHash { get; private set; }
        public Email Email { get; private set; }
        public IReadOnlyList<Role> Roles => _roles.AsReadOnly();
        public ZonedDateTime CreatedAt { get; }

        public AuthUser(UserId id, Username username, PasswordHash passwordHash, Email email,
                ZonedDateTime createdAt, IReadOnlyList<Role> roles) {
            _roles = roles.ToList();
            Id = id;
            Username = username;
            PasswordHash = passwordHash;
            Email = email;
            CreatedAt = createdAt;
        }

        public bool Can(Permission permission) => Roles.Any(r => r.Permissions.Contains(permission));

        public void Register(Role registeredUserRole) {
            if (registeredUserRole.Rolename != Role.REGISTERED_USER_ROLENAME) {
                throw new ArgumentException("You're supposed to add the registered user role.", nameof(registeredUserRole));
            }

            if (Roles.Count != 1 || Roles.Single().Rolename != Role.PENDING_REGISTRATION_ROLENAME) {
                throw new ManualValidationException("This user has no pending registration.");
            }

            _roles.Clear();
            AssignRole(registeredUserRole);
        }

        public void AssignRole(Role role) {
            if (_roles.Contains(role)) {
                throw new ManualValidationException("This user already has that role.");
            }
            _roles.Add(role);
        }

        public void RemoveRole(Role role) {
            if (!_roles.Contains(role)) {
                throw new ManualValidationException("This user doesn't have that role.");
            }
            if (role.Rolename == Role.ADMIN_ROLENAME && PROTECTED_USERS.Contains(Username)) {
                throw new ManualValidationException("You can't remove the admin role from protected users.");
            }
            _roles.Remove(role);
        }
    }
}
