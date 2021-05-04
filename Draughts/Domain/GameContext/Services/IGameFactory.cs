using Draughts.Domain.GameContext.Models;
using Draughts.Repositories;
using static Draughts.Domain.GameContext.Services.GameFactory;

namespace Draughts.Domain.GameContext.Services {
    public interface IGameFactory {
        (Game game, GameState gameState) BuildGame(IIdPool idPool, GameSettings settings, UserInfo creator, Color creatorColor);
        Player BuildPlayer(IIdPool idPool, UserInfo user, Color color);
    }
}
