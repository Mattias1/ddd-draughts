using Draughts.Application.Auth.Services;
using Draughts.Common.Events;
using Draughts.Domain.AuthContext.Events;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using NodaTime;

namespace Draughts.Application.Auth;

public sealed class SynchronizePendingUserEventHandler : DomainEventHandler<AuthUserCreated> {
    private readonly UserRegistrationService _userRegistrationService;

    public SynchronizePendingUserEventHandler(IClock clock, EventsRepository eventsRepository, IUnitOfWork unitOfWork,
            UserRegistrationService userRegistrationService) : base(clock, eventsRepository, unitOfWork) {
        _userRegistrationService = userRegistrationService;
    }

    public override void Handle(AuthUserCreated evt) {
        HandleWithTransaction(TransactionDomain.User, evt, tran => {
            _userRegistrationService.CreateUser(evt.UserId, evt.Username);
        });
    }
}
