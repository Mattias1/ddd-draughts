using Draughts.Domain.GameAggregate.Models;

namespace Draughts.Repositories {
    public interface IGameRepository : IRepository<Game, GameId> {
    }
}
