using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;

namespace Draughts.Controllers.Lobby.Services {
    public interface IGameFactory {
        Game CreateGame(GameSettings settings, User creator, Color creatorColor);
    }
}