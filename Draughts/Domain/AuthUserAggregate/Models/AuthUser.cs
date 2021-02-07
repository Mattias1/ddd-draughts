using Draughts.Common;
using Draughts.Common.OoConcepts;
using Draughts.Domain.UserAggregate.Models;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Domain.AuthUserAggregate.Models {
    public class AuthUser : Entity<AuthUser, AuthUserId> {
        private readonly List<Role> _roles;

        public override AuthUserId Id { get; }
        public UserId UserId { get; }
        public Username Username { get; private set; }
        public PasswordHash PasswordHash { get; private set; }
        public Email Email { get; private set; }
        public IReadOnlyList<Role> Roles => _roles.AsReadOnly();
        public ZonedDateTime CreatedAt { get; }

        public AuthUser(AuthUserId id, UserId userId, Username username, PasswordHash passwordHash, Email email,
                ZonedDateTime createdAt, IReadOnlyList<Role> roles) {
            _roles = roles.ToList();
            Id = id;
            UserId = userId;
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
            _roles.Add(registeredUserRole);
        }
    }
}
