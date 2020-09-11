using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;

namespace Draughts.Application.Lobby.Services {
    public interface IGameFactory {
        Game CreateGame(GameSettings settings, User creator, Color creatorColor);
    }
}