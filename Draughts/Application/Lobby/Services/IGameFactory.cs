using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;

namespace Draughts.Application.Lobby.Services {
    public interface IGameFactory {
        Game CreateGame(IIdPool idPool, GameSettings settings, User creator, Color creatorColor);
        void JoinGame(IIdPool idPool, Game game, User user, Color color);
    }
}