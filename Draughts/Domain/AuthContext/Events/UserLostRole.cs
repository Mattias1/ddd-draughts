using Draughts.Common.Events;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using System;

namespace Draughts.Domain.AuthContext.Events;

public sealed class UserLostRole : DomainEvent {
    public const string TYPE = "role.lost";

    public UserId UserId { get; }
    public Username Username { get; }
    public RoleId RoleId { get; }
    public string Rolename { get; }
    public UserId RemovedBy { get; }

    public UserLostRole(AuthUser user, RoleId roleId, string rolename, UserId removedBy, DomainEventId id, ZonedDateTime created) : base(id, TYPE, created) {
        UserId = user.Id;
        Username = user.Username;
        RoleId = roleId;
        Rolename = rolename;
        RemovedBy = removedBy;
    }

    public static DomainEventFactory Factory(AuthUser user, RoleId roleId, string rolename, UserId removedBy) {
        return (id, now) => new UserLostRole(user, roleId, rolename, removedBy, id, now);
    }
}
