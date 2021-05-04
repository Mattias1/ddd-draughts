using Draughts.Common.Events;
using Draughts.Application.Auth.Services;
using Draughts.Domain.AuthUserContext.Events;

namespace Draughts.Application.Auth {
    public class FinishUserRegistrationEventHandler : DomainEventHandler<UserCreated> {
        private readonly IUserRegistrationService _userRegistrationService;

        public FinishUserRegistrationEventHandler(IUserRegistrationService userRegistrationService) {
            _userRegistrationService = userRegistrationService;
        }

        public override void Handle(UserCreated evt) {
            _userRegistrationService.FinishRegistration(evt.UserId);
        }
    }
}
