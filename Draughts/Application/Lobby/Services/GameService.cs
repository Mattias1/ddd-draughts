using Draughts.Common;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.GameContext.Services;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using System.Linq;
using static Draughts.Domain.GameContext.Services.GameFactory;

namespace Draughts.Application.Lobby.Services {
    // Note: This name is way to generic. In the future I'll put everything in here. So I'll rename it then :)
    public class GameService : IGameService {
        private readonly IGameFactory _gameFactory;
        private readonly IGameRepository _gameRepository;
        private readonly IGameStateRepository _gameStateRepository;
        private readonly IIdGenerator _idGenerator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public GameService(IGameFactory gameFactory, IGameRepository gameRepository,
                IGameStateRepository gameStateRepository, IIdGenerator idGenerator,
                IUnitOfWork unitOfWork, IUserRepository userRepository) {
            _gameFactory = gameFactory;
            _gameRepository = gameRepository;
            _gameStateRepository = gameStateRepository;
            _idGenerator = idGenerator;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        public Game CreateGame(UserId userId, GameSettings gameSettings, Color joinColor) {
            var user = _unitOfWork.WithUserTransaction(tran => {
                var user = _userRepository.FindById(userId);
                return tran.CommitWith(user);
            });

            return _unitOfWork.WithGameTransaction(tran => {
                var idPool = _idGenerator.ReservePool();
                var userInfo = new UserInfo(user.Id, user.Username, user.Rank);
                var (game, gameState) = _gameFactory.BuildGame(idPool, gameSettings, userInfo, joinColor);

                _gameRepository.Save(game);
                _gameStateRepository.Save(gameState);

                return tran.CommitWith(game);
            });
        }

        public void JoinGame(UserId userId, GameId gameId, Color? color) {
            var user = _unitOfWork.WithUserTransaction(tran => {
                var user = _userRepository.FindById(userId);
                return tran.CommitWith(user);
            });

            _unitOfWork.WithGameTransaction(tran => {
                var game = _gameRepository.FindByIdOrNull(gameId);

                if (game is null) {
                    throw new ManualValidationException("Game not found");
                }

                var idPool = _idGenerator.ReservePool();
                var userInfo = new UserInfo(user.Id, user.Username, user.Rank);
                var player = _gameFactory.BuildPlayer(idPool, userInfo, color ?? GetRemainingColor(game));
                game.JoinGame(player, player.CreatedAt);

                _gameRepository.Save(game);

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
