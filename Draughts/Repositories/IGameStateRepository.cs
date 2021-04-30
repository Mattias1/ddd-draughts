using Draughts.Domain.GameAggregate.Models;

namespace Draughts.Repositories {
    public interface IGameStateRepository : IRepository<GameState, GameId> {
    }
}
