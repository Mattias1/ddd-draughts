using Draughts.Domain.GameAggregate.Models;

namespace Draughts.Repositories {
    public interface IGameRepository : IRepository<Game> {
        Game FindById(GameId id);
        Game? FindByIdOrNull(GameId id);
    }
}
