using Draughts.Common.Events;
using Draughts.Application.Auth.Services;
using Draughts.Domain.AuthContext.Events;

namespace Draughts.Application.Auth {
    public class FinishUserRegistrationEventHandler : DomainEventHandler<UserCreated> {
        private readonly UserRegistrationService _userRegistrationService;

        public FinishUserRegistrationEventHandler(UserRegistrationService userRegistrationService) {
            _userRegistrationService = userRegistrationService;
        }

        public override void Handle(UserCreated evt) {
            _userRegistrationService.FinishRegistration(evt.UserId);
        }
    }
}
