using Draughts.Domain.GameContext.Models;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.InMemory;

public sealed class InMemoryGameStateRepository : InMemoryRepository<GameState, GameId>, IGameStateRepository {
    public InMemoryGameStateRepository(IRepositoryUnitOfWork unitOfWork) : base(unitOfWork) { }

    protected override IList<GameState> GetBaseQuery() {
        var settings = GameDatabase.Get.GamesTable.ToDictionary(g => g.Id, g => g.GetGameSettings());
        var moves = GameDatabase.Get.MovesTable.ToLookup(m => m.GameId);
        return GameDatabase.Get.GameStatesTable
            .Select(g => g.ToDomainModel(settings[g.Id], moves[g.Id]))
            .ToList();
    }

    protected override void SaveInternal(GameState entity) {
        var dbGameState = DbGameState.FromDomainModel(entity);
        UnitOfWork.Store(dbGameState, tran => GameDatabase.Temp(tran).GameStatesTable);

        var maxDbIndex = GameDatabase.Get.MovesTable
            .Where(m => m.GameId == entity.Id.Value)
            .Select(m => (int?)m.Index)
            .Max() ?? -1;
        foreach (var dbMove in DbMove.ArrayFromDomainModels(entity, maxDbIndex)) {
            UnitOfWork.Store(dbMove, tran => GameDatabase.Temp(tran).MovesTable);
        }
    }
}
