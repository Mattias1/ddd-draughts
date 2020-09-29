using Draughts.Common.Utilities;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;
using NodaTime;

namespace Draughts.Application.Lobby.Services {
    public class GameFactory : IGameFactory {
        private readonly IClock _clock;
        private readonly IGameRepository _gameRepository;
        private readonly IIdGenerator _idGenerator;
        private readonly IPlayerRepository _playerRepository;

        public GameFactory(IClock clock, IGameRepository gameRepository,
            IIdGenerator idGenerator, IPlayerRepository playerRepository
        ) {
            _clock = clock;
            _gameRepository = gameRepository;
            _idGenerator = idGenerator;
            _playerRepository = playerRepository;
        }

        public Game CreateGame(GameSettings settings, User creator, Color creatorColor) {
            var nextId = new GameId(_idGenerator.Next());

            var player = BuildPlayer(creator, creatorColor);
            var game = new Game(nextId, settings, _clock.UtcNow());
            game.JoinGame(player, _clock.UtcNow());

            _playerRepository.Save(player);
            _gameRepository.Save(game);

            return game;
        }

        // TODO: Do I want the JoinGame method in a factory? Should I rename this class? Should I move this method?
        public void JoinGame(Game game, User user, Color color) {
            var player = BuildPlayer(user, color);
            game.JoinGame(player, _clock.UtcNow());

            _playerRepository.Save(player);
            _gameRepository.Save(game);
        }

        public Player BuildPlayer(User user, Color color) {
            var nextPlayerId = new PlayerId(_idGenerator.Next());
            return new Player(nextPlayerId, user.Id, user.Username, user.Rank, color);
        }
    }
}
