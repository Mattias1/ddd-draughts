using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;

namespace Draughts.Services {
    public interface IGameService {
        void CreateGame(UserId userId, GameSettings gameSettings, Color joinColor);
    }
}