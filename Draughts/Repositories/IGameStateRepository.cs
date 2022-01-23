using Draughts.Domain.GameContext.Models;

namespace Draughts.Repositories;

public interface IGameStateRepository : IRepository<GameState, GameId> {
}
