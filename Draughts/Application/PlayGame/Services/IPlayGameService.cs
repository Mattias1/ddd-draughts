using Draughts.Domain.GameAggregate.Models;

namespace Draughts.Application.PlayGame.Services {
    public interface IPlayGameService {
        void DoMove(GameId gameId, SquareNumber From, SquareNumber To);
    }
}
