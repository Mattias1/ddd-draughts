using Draughts.Domain.GameContext.Models;
using Draughts.Domain.UserContext.Models;

namespace Draughts.Application.Lobby.Services {
    public interface IGameService {
        Game CreateGame(UserId userId, GameSettings gameSettings, Color joinColor);
        void JoinGame(UserId userId, GameId gameId, Color? color);
    }
}