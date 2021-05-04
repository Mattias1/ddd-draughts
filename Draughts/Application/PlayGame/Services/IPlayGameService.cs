using Draughts.Domain.GameContext.Models;
using Draughts.Domain.UserContext.Models;

namespace Draughts.Application.PlayGame.Services {
    public interface IPlayGameService {
        void DoMove(UserId currentUser, GameId gameId, SquareId From, SquareId To);
        (Game game, GameState gameState) FindGameAndState(GameId gameId);
    }
}
