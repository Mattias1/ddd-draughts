using Draughts.Domain.GameContext.Models;
using NodaTime;
using System.Collections.Generic;

namespace Draughts.Repositories;

public interface IGameRepository : IRepository<Game, GameId> {
    IReadOnlyList<GameId> ListGameIdsForExpiredTurns(ZonedDateTime datetime);
}
