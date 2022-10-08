using Draughts.Common.Events;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using NodaTime;

namespace Draughts.Domain.AuthContext.Events;

public sealed class UserCreated : DomainEvent {
    public const string TYPE = "user.created";

    public UserId UserId { get; }
    public Username Username { get; }

    public UserCreated(UserId userId, Username username, DomainEventId id, ZonedDateTime createdAt,
            ZonedDateTime? handledAt) : base(id, TYPE, createdAt, handledAt) {
        UserId = userId;
        Username = username;
    }

    public override string BuildDataString() {
        return JsonUtils.SerializeEvent(Id, new EventData(UserId.Value, Username.Value));
    }

    public static DomainEventFactory Factory(User user) {
        return (id, now) => new UserCreated(user.Id, user.Username, id, now, null);
    }

    public static UserCreated FromStorage(DomainEventId id,
            ZonedDateTime createdAt, ZonedDateTime? handledAt, string dataString) {
        var data = JsonUtils.DeserializeEvent<EventData>(id, dataString);
        return new UserCreated(new UserId(data.UserId), new Username(data.Username), id, createdAt, handledAt);
    }

    private readonly record struct EventData(long UserId, string Username);
}
