using Draughts.Common;
using Draughts.Common.OoConcepts;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Events;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories;
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

    public void AssignRole(RoleId roleId, string rolename, UserId? responsibleUserId) {
        if (_rolesIds.Contains(roleId)) {
            throw new ManualValidationException("This user already has that role.");
        }
        _rolesIds.Add(roleId);

        if (responsibleUserId is not null) {
            RegisterEvent(UserGainedRole.Factory(this, roleId, rolename, responsibleUserId));
        }
    }

    public void RemoveRole(RoleId roleId, string rolename, UserId? responsibleUserId) {
        if (!_rolesIds.Contains(roleId)) {
            throw new ManualValidationException("This user doesn't have that role.");
        }
        _rolesIds.Remove(roleId);

        if (responsibleUserId is not null) {
            RegisterEvent(UserLostRole.Factory(this, roleId, rolename, responsibleUserId));
        }
    }

    public static AuthUser CreateNew(IIdPool idPool, Username username, Email email,
            string? plaintextPassword, RoleId pendingRegistrationRoleId, IClock clock) {
        var nextUserId = new UserId(idPool.NextForUser());
        var passwordHash = PasswordHash.Generate(plaintextPassword, nextUserId, username);

        var authUser = new AuthUser(nextUserId, username, passwordHash, email, clock.UtcNow(),
            new[] { pendingRegistrationRoleId });

        authUser.RegisterEvent(AuthUserCreated.Factory(authUser));
        return authUser;
    }
}
