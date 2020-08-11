using Draughts.Common.Events;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using NodaTime;

namespace Draughts.Domain.AuthUserAggregate.Events {
    public class UserCreated : DomainEvent {
        public const string TYPE = "user.created";

        public AuthUserId AuthUserId { get; }
        public UserId UserId { get; }
        public Username Username { get; }

        public UserCreated(User user, DomainEventId id, ZonedDateTime createdAt) : base(id, TYPE, createdAt) {
            AuthUserId = user.AuthUserId;
            UserId = user.Id;
            Username = user.Username;
        }
    }
}
