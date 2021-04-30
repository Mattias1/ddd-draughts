using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;

namespace Draughts.Domain.GameAggregate.Services {
    public interface IPlayGameDomainService {
        void DoMove(Game game, GameState gameState, UserId currentUserId, SquareId from, SquareId to);
    }
}
