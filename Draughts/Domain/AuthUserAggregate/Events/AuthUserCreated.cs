using Draughts.Common.Events;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;

namespace Draughts.Domain.AuthUserAggregate.Events {
    public class AuthUserCreated : DomainEvent {
        public const string TYPE = "authuser.created";

        public AuthUserId AuthUserId { get; }
        public UserId UserId { get; }
        public Username Username { get; }

        public AuthUserCreated(AuthUser authUser) : base(TYPE) {
            AuthUserId = authUser.Id;
            UserId = authUser.UserId;
            Username = authUser.Username;
        }
    }
}
