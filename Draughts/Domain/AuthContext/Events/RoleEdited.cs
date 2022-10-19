using Draughts.Common.Events;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories.Transaction;
using NodaTime;

namespace Draughts.Domain.AuthContext.Events;

public sealed class RoleEdited : DomainEvent {
    public const string TYPE = "role.edited";

    public RoleId RoleId { get; }
    public string Rolename { get; }
    public UserId EditedBy { get; }

    public override TransactionDomain OriginTransactionDomain => TransactionDomain.Auth;

    public RoleEdited(RoleId roleId, string rolename, UserId editedBy, DomainEventId id, ZonedDateTime createdAt,
            ZonedDateTime? handledAt) : base(id, TYPE, createdAt, handledAt) {
        RoleId = roleId;
        Rolename = rolename;
        EditedBy = editedBy;
    }

    public override string BuildDataString() {
        return JsonUtils.SerializeEvent(Id, new EventData(RoleId.Value, Rolename, EditedBy.Value));
    }

    public static DomainEventFactory Factory(Role role, UserId createdBy) {
        return (id, now) => new RoleEdited(role.Id, role.Rolename, createdBy, id, now, null);
    }

    public static RoleEdited FromStorage(DomainEventId id,
            ZonedDateTime createdAt, ZonedDateTime? handledAt, string dataString) {
        var data = JsonUtils.DeserializeEvent<EventData>(id, dataString);
        return new RoleEdited(new RoleId(data.RoleId), data.Rolename,
            new UserId(data.EditedBy), id, createdAt, handledAt);
    }

    private readonly record struct EventData(long RoleId, string Rolename, long EditedBy);
}
