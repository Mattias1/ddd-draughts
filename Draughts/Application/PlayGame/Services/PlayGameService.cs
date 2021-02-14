using Draughts.Common;
using Draughts.Common.Utilities;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using NodaTime;

namespace Draughts.Application.PlayGame.Services {
    public class PlayGameService : IPlayGameService {
        private readonly IClock _clock;
        private readonly IGameRepository _gameRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PlayGameService(IClock clock, IGameRepository gameRepository, IUnitOfWork unitOfWork) {
            _clock = clock;
            _gameRepository = gameRepository;
            _unitOfWork = unitOfWork;
        }

        public void DoMove(UserId currentUser, GameId gameId, SquareId from, SquareId to) {
            _unitOfWork.WithTransaction(TransactionDomain.Game, tran => {
                var game = _gameRepository.FindByIdOrNull(gameId) ?? throw new ManualValidationException("Game not found.");
                game.DoMove(currentUser, from, to, _clock.UtcNow());

                _gameRepository.Save(game);

                tran.Commit();
            });
        }
    }
}
