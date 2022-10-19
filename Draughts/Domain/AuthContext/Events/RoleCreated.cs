using Draughts.Common.Events;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories.Transaction;
using NodaTime;

namespace Draughts.Domain.AuthContext.Events;

public sealed class RoleCreated : DomainEvent {
    public const string TYPE = "role.created";

    public RoleId RoleId { get; }
    public string Rolename { get; }
    public UserId CreatedBy { get; }

    public override TransactionDomain OriginTransactionDomain => TransactionDomain.Auth;

    public RoleCreated(Role role, UserId createdBy, DomainEventId id, ZonedDateTime created)
            : this(role.Id, role.Rolename, createdBy, id, created, null) { }
    private RoleCreated(RoleId roleId, string rolename, UserId createdBy, DomainEventId id, ZonedDateTime created,
            ZonedDateTime? handledAt) : base(id, TYPE, created, handledAt) {
        RoleId = roleId;
        Rolename = rolename;
        CreatedBy = createdBy;
    }

    public override string BuildDataString() {
        return JsonUtils.SerializeEvent(Id, new EventData(RoleId.Value, Rolename, CreatedBy.Value));
    }

    public static DomainEventFactory Factory(Role role, UserId createdBy) {
        return (id, now) => new RoleCreated(role, createdBy, id, now);
    }

    public static RoleCreated FromStorage(DomainEventId id,
            ZonedDateTime createdAt, ZonedDateTime? handledAt, string dataString) {
        var data = JsonUtils.DeserializeEvent<EventData>(id, dataString);
        return new RoleCreated(new RoleId(data.RoleId), data.Rolename, new UserId(data.CreatedBy), id, createdAt, handledAt);
    }

    // TODO: Check if this is faster than using an ordinary struct
    //       Just jsonify a couple million in a benchmark or something
    private readonly record struct EventData(long RoleId, string Rolename, long CreatedBy);
}
