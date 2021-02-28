using Draughts.Domain.GameAggregate.Models;

namespace Draughts.Repositories {
    public interface IPlayerRepository : IRepository<Player, PlayerId> {
        void Save(Player entity, GameId gameId);
    }
}
