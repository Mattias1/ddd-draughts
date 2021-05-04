using Draughts.Common.Events;
using Draughts.Application.Auth.Services;
using Draughts.Domain.AuthUserContext.Events;

namespace Draughts.Application.Auth {
    public class SynchronizePendingUserEventHandler : DomainEventHandler<AuthUserCreated> {
        private readonly UserRegistrationService _userRegistrationService;

        public SynchronizePendingUserEventHandler(UserRegistrationService userRegistrationService) {
            _userRegistrationService = userRegistrationService;
        }

        public override void Handle(AuthUserCreated evt) {
            _userRegistrationService.CreateUser(evt.UserId, evt.Username);
        }
    }
}
