using Draughts.Application.Auth.Services;
using Draughts.Common.Events;
using Draughts.Domain.AuthContext.Events;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace Draughts.Application.Auth;

public sealed class FinishUserRegistrationEventHandler : DomainEventHandler<UserCreated> {
    private readonly UserRegistrationService _userRegistrationService;

    public FinishUserRegistrationEventHandler(IClock clock, EventsRepository eventsRepository,
            ILogger<FinishUserRegistrationEventHandler> logger, IUnitOfWork unitOfWork,
            UserRegistrationService userRegistrationService) : base(clock, eventsRepository, logger, unitOfWork) {
        _userRegistrationService = userRegistrationService;
    }

    public override void Handle(UserCreated evt) {
        HandleWithTransaction(TransactionDomain.Auth, evt, tran => {
            _userRegistrationService.FinishRegistration(evt.UserId);
        });
    }
}
