using Draughts.Common.Events;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using System;

namespace Draughts.Domain.AuthContext.Events;

public class RoleCreated : DomainEvent {
    public const string TYPE = "role.created";

    public RoleId RoleId { get; }
    public string Rolename { get; }
    public UserId CreatedBy { get; }

    public RoleCreated(Role role, UserId createdBy, DomainEventId id, ZonedDateTime created) : base(id, TYPE, created) {
        RoleId = role.Id;
        Rolename = role.Rolename;
        CreatedBy = createdBy;
    }

    public static Func<DomainEventId, ZonedDateTime, RoleCreated> Factory(Role role, UserId createdBy) {
        return (id, now) => new RoleCreated(role, createdBy, id, now);
    }
}
