using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;

namespace Draughts.Application.Lobby.Services {
    public interface IGameService {
        void CreateGame(UserId userId, GameSettings gameSettings, Color joinColor);
        void JoinGame(UserId userId, GameId gameId, Color? color);
    }
}