using Draughts.Common.Utilities;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;
using NodaTime;

namespace Draughts.Application.Lobby.Services {
    public class GameFactory : IGameFactory {
        private readonly IClock _clock;
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerRepository _playerRepository;

        public GameFactory(IClock clock, IGameRepository gameRepository,
                IPlayerRepository playerRepository) {
            _clock = clock;
            _gameRepository = gameRepository;
            _playerRepository = playerRepository;
        }

        public Game CreateGame(IIdPool idPool, GameSettings settings, User creator, Color creatorColor) {
            var nextId = new GameId(idPool.Next());
            var game = new Game(nextId, settings, _clock.UtcNow());

            var player = BuildPlayer(idPool, creator, creatorColor);
            game.JoinGame(player, _clock.UtcNow());

            _gameRepository.Save(game);
            _playerRepository.Save(player, game.Id);

            return game;
        }

        // TODO: Do I want the JoinGame method in a factory? Should I rename this class? Should I move this method?
        public void JoinGame(IIdPool idPool, Game game, User user, Color color) {
            var player = BuildPlayer(idPool, user, color);
            game.JoinGame(player, _clock.UtcNow());

            _gameRepository.Save(game);
            _playerRepository.Save(player, game.Id);
        }

        public Player BuildPlayer(IIdPool idPool, User user, Color color) {
            var nextPlayerId = new PlayerId(idPool.Next());
            return new Player(nextPlayerId, user.Id, user.Username, user.Rank, color, _clock.UtcNow());
        }
    }
}
