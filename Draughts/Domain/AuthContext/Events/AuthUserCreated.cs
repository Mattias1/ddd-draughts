using Draughts.Common.Events;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories.Transaction;
using NodaTime;

namespace Draughts.Domain.AuthContext.Events;

public sealed class AuthUserCreated : DomainEvent {
    public const string TYPE = "authuser.created";

    public UserId UserId { get; }
    public Username Username { get; }

    public override TransactionDomain OriginTransactionDomain => TransactionDomain.Auth;

    public AuthUserCreated(UserId userId, Username username, DomainEventId id, ZonedDateTime createdAt,
            ZonedDateTime? handledAt) : base(id, TYPE, createdAt, handledAt) {
        UserId = userId;
        Username = username;
    }

    public override string BuildDataString() {
        return JsonUtils.SerializeEvent(Id, new EventData(UserId.Value, Username.Value));
    }

    public static DomainEventFactory Factory(AuthUser authUser) {
        return (id, now) => new AuthUserCreated(authUser.Id, authUser.Username, id, now, null);
    }

    public static AuthUserCreated FromStorage(DomainEventId id,
            ZonedDateTime createdAt, ZonedDateTime? handledAt, string dataString) {
        var data = JsonUtils.DeserializeEvent<EventData>(id, dataString);
        return new AuthUserCreated(new UserId(data.UserId), new Username(data.Username), id, createdAt, handledAt);
    }

    private readonly record struct EventData(long UserId, string Username);
}
