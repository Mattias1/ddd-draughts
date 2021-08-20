using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories;
using NodaTime;

namespace Draughts.Domain.GameContext.Services {
    public class GameFactory : IGameFactory {
        private readonly IClock _clock;

        public GameFactory(IClock clock) {
            _clock = clock;
        }

        public (Game game, GameState gameState) BuildGame(
                IIdPool idPool, GameSettings settings, UserInfo creator, Color creatorColor) {
            var nextGameId = new GameId(idPool.NextForGame());
            var game = new Game(nextGameId, settings, _clock.UtcNow());
            var gameState = GameState.InitialState(game.Id, settings.BoardSize);

            var player = BuildPlayer(idPool, creator, creatorColor);
            game.JoinGame(player, _clock.UtcNow());

            return (game, gameState);
        }

        public Player BuildPlayer(IIdPool idPool, UserInfo user, Color color) {
            var nextPlayerId = new PlayerId(idPool.Next());
            return new Player(nextPlayerId, user.Id, user.Username, user.Rank, color, _clock.UtcNow());
        }

        public record UserInfo(UserId Id, Username Username, Rank Rank);
    }
}
