using Draughts.Common.Events;
using Draughts.Domain.AuthContext.Events;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;

namespace Draughts.Application.Auth;

public sealed class FinishedGameEventHandler : DomainEventHandler<GameFinished> {
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserRepository _userRepository;

    public FinishedGameEventHandler(IUnitOfWork unitOfWork, UserRepository userRepository) {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    public override void Handle(GameFinished evt) {
        _unitOfWork.WithUserTransaction(tran => {
            foreach (var userId in evt.Players) {
                var user = _userRepository.FindById(userId);
                user.UpdateStatisticsForFinishedGame(evt.SettingsPreset, evt.Victor);
                _userRepository.Save(user);
            }
        });
    }
}
