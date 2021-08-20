using Draughts.Common.Events;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using NodaTime;
using System;

namespace Draughts.Domain.AuthContext.Events {
    public class UserCreated : DomainEvent {
        public const string TYPE = "user.created";

        public UserId UserId { get; }
        public Username Username { get; }

        public UserCreated(User user, DomainEventId id, ZonedDateTime createdAt) : base(id, TYPE, createdAt) {
            UserId = user.Id;
            Username = user.Username;
        }

        public static Func<DomainEventId, ZonedDateTime, UserCreated> Factory(User user) {
            return (id, now) => new UserCreated(user, id, now);
        }
    }
}
