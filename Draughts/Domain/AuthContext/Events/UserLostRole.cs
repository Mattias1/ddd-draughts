using Draughts.Common.Events;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories.Transaction;
using NodaTime;

namespace Draughts.Domain.AuthContext.Events;

public sealed class UserLostRole : DomainEvent {
    public const string TYPE = "role.lost";

    public UserId UserId { get; }
    public Username Username { get; }
    public RoleId RoleId { get; }
    public string Rolename { get; }
    public UserId RemovedBy { get; }

    public override TransactionDomain OriginTransactionDomain => TransactionDomain.Auth;

    public UserLostRole(UserId userId, Username username, RoleId roleId, string rolename, UserId removedBy,
            DomainEventId id, ZonedDateTime createdAt, ZonedDateTime? handledAt) : base(id, TYPE, createdAt, handledAt) {
        UserId = userId;
        Username = username;
        RoleId = roleId;
        Rolename = rolename;
        RemovedBy = removedBy;
    }

    public override string BuildDataString() {
        return JsonUtils.SerializeEvent(Id, new EventData(UserId.Value, Username.Value,
            RoleId.Value, Rolename, RemovedBy.Value));
    }

    public static DomainEventFactory Factory(AuthUser user, RoleId roleId, string rolename, UserId removedBy) {
        return (id, now) => new UserLostRole(user.Id, user.Username, roleId, rolename, removedBy, id, now, null);
    }

    public static UserGainedRole FromStorage(DomainEventId id,
            ZonedDateTime createdAt, ZonedDateTime? handledAt, string dataString) {
        var data = JsonUtils.DeserializeEvent<EventData>(id, dataString);
        return new UserGainedRole(new UserId(data.UserId), new Username(data.Username),
            new RoleId(data.RoleId), data.Rolename, new UserId(data.RemovedBy), id, createdAt, handledAt);
    }

    private readonly record struct EventData(long UserId, string Username,
            long RoleId, string Rolename, long RemovedBy);
}
