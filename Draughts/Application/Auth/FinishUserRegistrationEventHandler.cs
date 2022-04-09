using Draughts.Application.Auth.Services;
using Draughts.Common.Events;
using Draughts.Domain.AuthContext.Events;

namespace Draughts.Application.Auth;

public sealed class FinishUserRegistrationEventHandler : DomainEventHandler<UserCreated> {
    private readonly UserRegistrationService _userRegistrationService;

    public FinishUserRegistrationEventHandler(UserRegistrationService userRegistrationService) {
        _userRegistrationService = userRegistrationService;
    }

    public override void Handle(UserCreated evt) {
        _userRegistrationService.FinishRegistration(evt.UserId);
    }
}
