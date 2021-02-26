using Draughts.Common.Events;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using NodaTime;

namespace Draughts.Domain.AuthUserAggregate.Events {
    public class AuthUserCreated : DomainEvent {
        public const string TYPE = "authuser.created";

        public UserId UserId { get; }
        public Username Username { get; }

        public AuthUserCreated(AuthUser authUser, DomainEventId id, ZonedDateTime created) : base(id, TYPE, created) {
            UserId = authUser.Id;
            Username = authUser.Username;
        }
    }
}
