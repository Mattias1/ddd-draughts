using Draughts.Common.Events;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using NodaTime;

namespace Draughts.Domain.AuthContext.Events;

public sealed class UserGainedRole : DomainEvent {
    public const string TYPE = "role.gained";

    public UserId UserId { get; }
    public Username Username { get; }
    public RoleId RoleId { get; }
    public string Rolename { get; }
    public UserId AssignedBy { get; }

    public UserGainedRole(AuthUser user, RoleId roleId, string rolename, UserId assignedBy, DomainEventId id,
            ZonedDateTime created) : base(id, TYPE, created) {
        UserId = user.Id;
        Username = user.Username;
        RoleId = roleId;
        Rolename = rolename;
        AssignedBy = assignedBy;
    }

    public static DomainEventFactory Factory(AuthUser user, RoleId roleId, string rolename, UserId assignedBy) {
        return (id, now) => new UserGainedRole(user, roleId, rolename, assignedBy, id, now);
    }
}
