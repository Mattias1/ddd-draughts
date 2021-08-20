using Draughts.Common;
using Draughts.Common.OoConcepts;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Domain.AuthUserContext.Models {
    public class AuthUser : Entity<AuthUser, UserId> {
        public static IReadOnlyList<string> PROTECTED_USERS => new [] { Username.ADMIN, Username.MATTY };

        private readonly List<RoleId> _rolesIds;

        public override UserId Id { get; }
        public Username Username { get; private set; }
        public PasswordHash PasswordHash { get; private set; }
        public Email Email { get; private set; }
        public IReadOnlyList<RoleId> RoleIds => _rolesIds.AsReadOnly();
        public ZonedDateTime CreatedAt { get; }

        public AuthUser(UserId id, Username username, PasswordHash passwordHash, Email email,
                ZonedDateTime createdAt, IEnumerable<RoleId> roles) {
            _rolesIds = roles.ToList();
            Id = id;
            Username = username;
            PasswordHash = passwordHash;
            Email = email;
            CreatedAt = createdAt;
        }

        public void AssignRole(RoleId roleId) {
            if (_rolesIds.Contains(roleId)) {
                throw new ManualValidationException("This user already has that role.");
            }
            _rolesIds.Add(roleId);
        }

        public void RemoveRole(RoleId roleId) {
            if (!_rolesIds.Contains(roleId)) {
                throw new ManualValidationException("This user doesn't have that role.");
            }
            _rolesIds.Remove(roleId);
        }
    }
}
