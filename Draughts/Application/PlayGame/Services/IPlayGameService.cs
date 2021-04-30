using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;

namespace Draughts.Application.PlayGame.Services {
    public interface IPlayGameService {
        void DoMove(UserId currentUser, GameId gameId, SquareId From, SquareId To);
        (Game game, GameState gameState) FindGameAndState(GameId gameId);
    }
}
