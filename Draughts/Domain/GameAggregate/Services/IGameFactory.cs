using Draughts.Domain.GameAggregate.Models;
using Draughts.Repositories;
using static Draughts.Domain.GameAggregate.Services.GameFactory;

namespace Draughts.Domain.GameAggregate.Services {
    public interface IGameFactory {
        (Game game, GameState gameState) BuildGame(IIdPool idPool, GameSettings settings, UserInfo creator, Color creatorColor);
        Player BuildPlayer(IIdPool idPool, UserInfo user, Color color);
    }
}