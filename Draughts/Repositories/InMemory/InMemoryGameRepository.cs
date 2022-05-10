using Draughts.Domain.GameContext.Models;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using NodaTime;
using SqlQueryBuilder.Common;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.InMemory;

public sealed class InMemoryGameRepository : InMemoryRepository<Game, GameId>, IGameRepository {
    public InMemoryGameRepository(IRepositoryUnitOfWork unitOfWork) : base(unitOfWork) { }

    public IReadOnlyList<GameId> ListGameIdsForExpiredTurns(ZonedDateTime datetime) {
        return GetBaseQuery()
            .Where(g => g.Turn is not null && g.Turn.ExpiresAt.ToInstant() < datetime.ToInstant())
            .MapReadOnly(g => g.Id);
    }

    protected override IList<Game> GetBaseQuery() {
        var players = GameDatabase.Get.PlayersTable.ToLookup(p => p.GameId, p => p.ToDomainModel());
        return GameDatabase.Get.GamesTable
            .Select(g => g.ToDomainModel(players[g.Id].ToList()))
            .ToList();
    }

    protected override void SaveInternal(Game entity) {
        var dbGame = DbGame.FromDomainModel(entity);
        UnitOfWork.Store(dbGame, tran => GameDatabase.Temp(tran).GamesTable);
        foreach (var dbPlayer in entity.Players.Select(p => DbPlayer.FromDomainModel(p, entity.Id))) {
            UnitOfWork.Store(dbPlayer, tran => GameDatabase.Temp(tran).PlayersTable);
        }
    }
}
