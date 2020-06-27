using Draughts.Common.Events;
using Draughts.Domain.AuthUserAggregate.Events;
using Draughts.Services;

namespace Draughts.EventHandlers {
    public class RegisterUserEventHandler : DomainEventHandler<UserCreated> {
        private readonly IAuthUserFactory _authUserFactory;

        public RegisterUserEventHandler(IAuthUserFactory authUserFactory) : base(UserCreated.TYPE) {
            _authUserFactory = authUserFactory;
        }

        public override void Handle(UserCreated evt) {
            _authUserFactory.FinishRegistration(evt.AuthUserId);
        }
    }
}
