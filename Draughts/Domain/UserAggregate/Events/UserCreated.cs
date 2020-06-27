using Draughts.Common.Events;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;

namespace Draughts.Domain.AuthUserAggregate.Events {
    public class UserCreated : DomainEvent {
        public const string TYPE = "user.created";

        public AuthUserId AuthUserId { get; }
        public UserId UserId { get; }
        public Username Username { get; }

        public UserCreated(User user) : base(TYPE) {
            AuthUserId = user.AuthUserId;
            UserId = user.Id;
            Username = user.Username;
        }
    }
}
