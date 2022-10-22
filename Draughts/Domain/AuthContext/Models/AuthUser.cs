using Draughts.Common;
using Draughts.Common.OoConcepts;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Events;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories.Misc;
using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Domain.AuthContext.Models;

public sealed class AuthUser : AggregateRoot<AuthUser, UserId> {
    public static IReadOnlyList<string> PROTECTED_USERS => new[] { Username.ADMIN, Username.MATTY };

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

    public void AssignRole(RoleId roleId, string rolename) {
        if (_rolesIds.Contains(roleId)) {
            throw new ManualValidationException($"This user ({Id}, {Username}) "
                + $"already has that role ({roleId}, {rolename}).");
        }
        _rolesIds.Add(roleId);
    }

    public void RemoveRole(RoleId roleId, string rolename) {
        if (rolename == Role.ADMIN_ROLENAME && AuthUser.PROTECTED_USERS.Contains(Username.Value)) {
            throw new ManualValidationException("You can't remove the admin role from protected users "
                + $"({Id}, {Username}).");
        }
        if (!_rolesIds.Contains(roleId)) {
            throw new ManualValidationException($"This user ({Id}, {Username}) "
                + "doesn't have that role ({roleId}, {rolename}).");
        }
        _rolesIds.Remove(roleId);
    }

    public static AuthUser CreateNew(IIdPool idPool, Username username, Email email,
            string? plaintextPassword, RoleId pendingRegistrationRoleId, IClock clock) {
        var nextUserId = new UserId(idPool.NextForUser());
        var passwordHash = PasswordHash.Generate(plaintextPassword, nextUserId, username);

        var authUser = new AuthUser(nextUserId, username, passwordHash, email, clock.UtcNow(),
            new[] { pendingRegistrationRoleId });

        authUser.AttachEvent(AuthUserCreated.Factory(authUser));
        return authUser;
    }
}
