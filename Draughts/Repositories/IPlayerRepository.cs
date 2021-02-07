using Draughts.Domain.GameAggregate.Models;

namespace Draughts.Repositories {
    public interface IPlayerRepository : IRepository<Player> {
        void Save(Player entity, GameId gameId);
    }
}
