using Draughts.Domain.GameContext.Models;
using Draughts.Domain.UserContext.Models;

namespace Draughts.Domain.GameContext.Services {
    public interface IPlayGameDomainService {
        void DoMove(Game game, GameState gameState, UserId currentUserId, SquareId from, SquareId to);
    }
}
