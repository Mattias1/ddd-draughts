using Draughts.Common;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;
using Draughts.Repositories.Databases;
using System.Linq;

namespace Draughts.Application.Lobby.Services {
    // Note: This name is way to generic. In the future I'll put everything in here. So I'll rename it then :)
    public class GameService : IGameService {
        private readonly IGameFactory _gameFactory;
        private readonly IGameRepository _gameRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public GameService(IGameFactory gameFactory, IGameRepository gameRepository,
            IUnitOfWork unitOfWork, IUserRepository userRepository
        ) {
            _gameFactory = gameFactory;
            _gameRepository = gameRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public Game CreateGame(UserId userId, GameSettings gameSettings, Color joinColor) {
            var user = _userRepository.FindById(userId);

            return _unitOfWork.WithTransaction(TransactionDomain.Game, tran => {
                var game = _gameFactory.CreateGame(gameSettings, user, joinColor);

                tran.Commit();
                return game;
            });
        }

        public void JoinGame(UserId userId, GameId gameId, Color? color) {
            var user = _userRepository.FindById(userId);

            _unitOfWork.WithTransaction(TransactionDomain.Game, tran => {
                var game = _gameRepository.FindByIdOrNull(gameId);

                if (game is null) {
                    throw new ManualValidationException("Game not found");
                }

                _gameFactory.JoinGame(game, user, color ?? GetRemainingColor(game));

                tran.Commit();
            });
        }

        private Color GetRemainingColor(Game game) {
            var player = game.Players.FirstOrDefault();
            if (player is null) {
                throw new ManualValidationException("This game has no players");
            }

            return player.Color.Other;
        }
    }
}
