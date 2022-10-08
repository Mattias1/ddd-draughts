using Draughts.Common.Events;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using NodaTime;

namespace Draughts.Domain.AuthContext.Events;

public sealed class RoleDeleted : DomainEvent {
    public const string TYPE = "role.deleted";

    public RoleId RoleId { get; }
    public string Rolename { get; }
    public UserId DeletedBy { get; }

    public RoleDeleted(RoleId roleId, string rolename, UserId deletedBy, DomainEventId id, ZonedDateTime createdAt,
            ZonedDateTime? handledAt) : base(id, TYPE, createdAt, handledAt) {
        RoleId = roleId;
        Rolename = rolename;
        DeletedBy = deletedBy;
    }

    public override string BuildDataString() {
        return JsonUtils.SerializeEvent(Id, new EventData(RoleId.Value, Rolename, DeletedBy.Value));
    }

    public static DomainEventFactory Factory(Role role, UserId createdBy) {
        return (id, now) => new RoleDeleted(role.Id, role.Rolename, createdBy, id, now, null);
    }

    public static RoleDeleted FromStorage(DomainEventId id,
            ZonedDateTime createdAt, ZonedDateTime? handledAt, string dataString) {
        var data = JsonUtils.DeserializeEvent<EventData>(id, dataString);
        return new RoleDeleted(new RoleId(data.RoleId), data.Rolename, new UserId(data.DeletedBy), id, createdAt, handledAt);
    }

    private readonly record struct EventData(long RoleId, string Rolename, long DeletedBy);
}
