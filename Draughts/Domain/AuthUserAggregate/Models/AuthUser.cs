using Draughts.Common;
using Draughts.Domain.UserAggregate.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Domain.AuthUserAggregate.Models {
    public class AuthUser {
        private readonly List<Role> _roles;

        public AuthUserId Id { get; }
        public UserId UserId { get; }
        public Username Username { get; private set; }
        public PasswordHash PasswordHash { get; private set; }
        public Email Email { get; private set; }
        public IReadOnlyList<Role> Roles => _roles.AsReadOnly();

        public AuthUser(AuthUserId id, UserId userId, Username username, PasswordHash passwordHash, Email email, IReadOnlyList<Role> roles) {
            _roles = roles.ToList();
            Id = id;
            UserId = userId;
            Username = username;
            PasswordHash = passwordHash;
            Email = email;
        }

        public bool Can(Permission permission) => Roles.Any(r => r.Permissions.Contains(permission));

        public override bool Equals(object? obj) => Equals(obj as AuthUser);
        public bool Equals(AuthUser? other) => other is null ? false : other.Id == Id;
        public override int GetHashCode() => Id.GetHashCode();
        public static bool operator ==(AuthUser? left, AuthUser? right) => Compare.NullSafeEquals(left, right);
        public static bool operator !=(AuthUser? left, AuthUser? right) => Compare.NullSafeNotEquals(left, right);

        public void Register(Role registeredUserRole) {
            if (registeredUserRole.Rolename != Role.REGISTERED_USER_ROLENAME) {
                throw new ArgumentException("You're supposed to add the registered user role.", nameof(registeredUserRole));
            }

            if (Roles.Count != 1 || Roles.Single().Rolename != Role.PENDING_REGISTRATION_ROLENAME) {
                throw new ManualValidationException("This user has no pending registration.");
            }

            _roles.Clear();
            _roles.Add(registeredUserRole);
        }
    }
}
