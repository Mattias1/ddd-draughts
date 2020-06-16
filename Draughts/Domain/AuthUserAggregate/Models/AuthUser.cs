using Draughts.Common;
using Draughts.Domain.UserAggregate.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Domain.AuthUserAggregate.Models {
    public class AuthUser {
        public AuthUserId Id { get; }
        public UserId UserId { get; }
        public Username Username { get; }
        public PasswordHash PasswordHash { get; }
        public Email Email { get; }
        public IReadOnlyList<Role> Roles { get; }

        public AuthUser(AuthUserId id, UserId userId, Username username, PasswordHash passwordHash, Email email, IReadOnlyList<Role> roles) {
            Id = id;
            UserId = userId;
            Username = username;
            PasswordHash = passwordHash;
            Email = email;
            Roles = roles;
        }

        public bool Can(Permission permission) => Roles.Any(r => r.Permissions.Contains(permission));

        public override bool Equals(object? obj) => Equals(obj as AuthUser);
        public bool Equals(AuthUser? other) => other is null ? false : other.Id == Id;
        public override int GetHashCode() => Id.GetHashCode();
        public static bool operator ==(AuthUser? left, AuthUser? right) => Compare.NullSafeEquals(left, right);
        public static bool operator !=(AuthUser? left, AuthUser? right) => Compare.NullSafeNotEquals(left, right);
    }
}
