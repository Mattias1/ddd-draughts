using Draughts.Common.Events;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories.Transaction;
using NodaTime;

namespace Draughts.Domain.AuthContext.Events;

public sealed class UserGainedRole : DomainEvent {
    public const string TYPE = "role.gained";

    public UserId UserId { get; }
    public Username Username { get; }
    public RoleId RoleId { get; }
    public string Rolename { get; }
    public UserId AssignedBy { get; }

    public override TransactionDomain OriginTransactionDomain => TransactionDomain.Auth;

    public UserGainedRole(UserId userId, Username username, RoleId roleId, string rolename, UserId assignedBy,
            DomainEventId id, ZonedDateTime createdAt, ZonedDateTime? handledAt) : base(id, TYPE, createdAt, handledAt) {
        UserId = userId;
        Username = username;
        RoleId = roleId;
        Rolename = rolename;
        AssignedBy = assignedBy;
    }

    public override string BuildDataString() {
        return JsonUtils.SerializeEvent(Id, new EventData(UserId.Value, Username.Value,
            RoleId.Value, Rolename, AssignedBy.Value));
    }

    public static DomainEventFactory Factory(AuthUser user, RoleId roleId, string rolename, UserId assignedBy) {
        return (id, now) => new UserGainedRole(user.Id, user.Username, roleId, rolename, assignedBy, id, now, null);
    }

    public static UserGainedRole FromStorage(DomainEventId id,
            ZonedDateTime createdAt, ZonedDateTime? handledAt, string dataString) {
        var data = JsonUtils.DeserializeEvent<EventData>(id, dataString);
        return new UserGainedRole(new UserId(data.UserId), new Username(data.Username),
            new RoleId(data.RoleId), data.Rolename, new UserId(data.AssignedBy), id, createdAt, handledAt);
    }

    private readonly record struct EventData(long UserId, string Username,
            long RoleId, string Rolename, long AssignedBy);
}
