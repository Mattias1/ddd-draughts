using Draughts.Common.Events;
using Draughts.Domain.AuthUserAggregate.Events;
using Draughts.Services;

namespace Draughts.EventHandlers {
    public class SynchronizePendingUserEventHandler : DomainEventHandler<AuthUserCreated> {
        private readonly IUserFactory _userFactory;

        public SynchronizePendingUserEventHandler(IUserFactory userFactory) : base(AuthUserCreated.TYPE) {
            _userFactory = userFactory;
        }

        public override void Handle(AuthUserCreated evt) {
            _userFactory.CreateUser(evt.AuthUserId, evt.UserId, evt.Username);
        }
    }
}
