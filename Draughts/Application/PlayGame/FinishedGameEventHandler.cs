using Draughts.Common.Events;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Events;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using NodaTime;

namespace Draughts.Application.Auth;

public sealed class FinishedGameEventHandler : DomainEventHandler<GameFinished> {
    private readonly UserRepository _userRepository;

    public FinishedGameEventHandler(IClock clock, EventsRepository eventsRepository, IUnitOfWork unitOfWork,
            UserRepository userRepository) : base(clock, eventsRepository, unitOfWork) {
        _userRepository = userRepository;
    }

    public override void Handle(GameFinished evt) {
        HandleWithTransaction(TransactionDomain.User, evt, tran => {
            foreach (var userId in evt.Players) {
                var user = _userRepository.FindById(userId);
                user.UpdateStatisticsForFinishedGame(evt.SettingsPreset, evt.Victor);
                _userRepository.Save(user);
            }
        });
    }
}
