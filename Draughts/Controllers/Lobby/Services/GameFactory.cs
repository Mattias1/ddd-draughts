using Draughts.Common.Utilities;
using Draughts.Controllers.Shared.Services;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;
using NodaTime;

namespace Draughts.Controllers.Lobby.Services {
    public class GameFactory : IGameFactory {
        private readonly IClock _clock;
        private readonly IEventFactory _eventFactory;
        private readonly IGameRepository _gameRepository;
        private readonly IIdGenerator _idGenerator;
        private readonly IPlayerRepository _playerRepository;

        public GameFactory(IClock clock,
            IEventFactory eventFactory, IGameRepository gameRepository,
            IIdGenerator idGenerator, IPlayerRepository playerRepository
        ) {
            _clock = clock;
            _eventFactory = eventFactory;
            _gameRepository = gameRepository;
            _idGenerator = idGenerator;
            _playerRepository = playerRepository;
        }

        public Game CreateGame(GameSettings settings, User creator, Color creatorColor) {
            var nextId = new GameId(_idGenerator.Next());
            var nextPlayerId = new PlayerId(_idGenerator.Next());

            var game = new Game(nextId, settings, _clock.UtcNow());
            var player = new Player(nextPlayerId, creator.Id, creator.Username, creator.Rank, creatorColor);
            game.JoinGame(player, _clock.UtcNow());

            _playerRepository.Save(player);
            _gameRepository.Save(game);

            _eventFactory.RaiseGameCreated(game, creator);

            return game;
        }
    }
}
