using Draughts.Common;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Events;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.GameContext.Services;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using NodaTime;

namespace Draughts.Application.PlayGame.Services {
    public class PlayGameService {
        private readonly IClock _clock;
        private readonly IGameRepository _gameRepository;
        private readonly IGameStateRepository _gameStateRepository;
        private readonly IPlayGameDomainService _playGameDomainService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVotingRepository _votingRepository;

        public PlayGameService(IClock clock, IGameRepository gameRepository, IGameStateRepository gameStateRepository,
                IPlayGameDomainService playGameDomainService, IUnitOfWork unitOfWork, IVotingRepository votingRepository) {
            _clock = clock;
            _gameRepository = gameRepository;
            _gameStateRepository = gameStateRepository;
            _playGameDomainService = playGameDomainService;
            _unitOfWork = unitOfWork;
            _votingRepository = votingRepository;
        }

        public void DoMove(UserId currentUserId, GameId gameId, SquareId from, SquareId to) {
            _unitOfWork.WithGameTransaction(tran => {
                var (game, gameState) = FindGameAndState(gameId);
                _playGameDomainService.DoMove(game, gameState, currentUserId, from, to);

                _gameRepository.Save(game);
                _gameStateRepository.Save(gameState);

                if (game.IsFinished) {
                    _unitOfWork.Raise(GameFinished.Factory(game));
                }
            });
        }

        public void VoteForDraw(UserId currentUserId, GameId gameId) {
            _unitOfWork.WithGameTransaction(tran => {
                var game = FindGame(gameId);
                var voting = _votingRepository.FindByIdOrNull(gameId) ?? Voting.StartNew(gameId);

                _playGameDomainService.VoteForDraw(game, voting, currentUserId);

                _votingRepository.Save(voting);
                _gameRepository.Save(game);

                if (game.IsFinished) {
                    _unitOfWork.Raise(GameFinished.Factory(game));
                }
            });
        }

        public void Resign(UserId currentUserId, GameId gameId) {
            _unitOfWork.WithGameTransaction(tran => {
                var game = FindGame(gameId);
                game.ResignGame(currentUserId, _clock.UtcNow());
                _gameRepository.Save(game);

                _unitOfWork.Raise(GameFinished.Factory(game));
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

        public Game FindGame(GameId gameId) {
            var game = _gameRepository.FindByIdOrNull(gameId);
            if (game is null) {
                throw new ManualValidationException("Game not found.");
            }
            return game;
        }
    }
}
