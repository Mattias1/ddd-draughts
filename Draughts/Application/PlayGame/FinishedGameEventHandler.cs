using Draughts.Common.Events;
using Draughts.Domain.AuthContext.Events;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace Draughts.Application.PlayGame;

public sealed class FinishedGameEventHandler : DomainEventHandler<GameFinished> {
    private readonly UserRepository _userRepository;

    public FinishedGameEventHandler(IClock clock, EventsRepository eventsRepository,
            ILogger<FinishedGameEventHandler> logger, IUnitOfWork unitOfWork,
            UserRepository userRepository) : base(clock, eventsRepository, logger, unitOfWork) {
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
