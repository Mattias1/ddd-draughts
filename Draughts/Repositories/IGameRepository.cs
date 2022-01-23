using Draughts.Domain.GameContext.Models;

namespace Draughts.Repositories;

public interface IGameRepository : IRepository<Game, GameId> {
}
