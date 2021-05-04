using Draughts.Common;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.GameContext.Services;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;

namespace Draughts.Application.PlayGame.Services {
    public class PlayGameService : IPlayGameService {
        private readonly IGameRepository _gameRepository;
        private readonly IGameStateRepository _gameStateRepository;
        private readonly IPlayGameDomainService _playGameDomainService;
        private readonly IUnitOfWork _unitOfWork;

        public PlayGameService(IGameRepository gameRepository, IGameStateRepository gameStateRepository,
                IPlayGameDomainService playGameDomainService, IUnitOfWork unitOfWork) {
            _gameRepository = gameRepository;
            _gameStateRepository = gameStateRepository;
            _playGameDomainService = playGameDomainService;
            _unitOfWork = unitOfWork;
        }

        public void DoMove(UserId currentUserId, GameId gameId, SquareId from, SquareId to) {
            _unitOfWork.WithGameTransaction(tran => {
                var (game, gameState) = FindGameAndState(gameId);
                _playGameDomainService.DoMove(game, gameState, currentUserId, from, to);

                _gameRepository.Save(game);
                _gameStateRepository.Save(gameState);

                tran.Commit();
            });
        }

        public (Game game, GameState gameState) FindGameAndState(GameId gameId) {
            var game = _gameRepository.FindByIdOrNull(gameId);
            var gameState = _gameStateRepository.FindByIdOrNull(gameId);
            if (game is null || gameState is null) {
                throw new ManualValidationException("Game not found.");
            }
            return (game, gameState);
        }
    }
}
