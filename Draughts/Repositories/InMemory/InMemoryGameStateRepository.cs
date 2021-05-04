using Draughts.Domain.GameContext.Models;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.InMemory {
    public class InMemoryGameStateRepository : InMemoryRepository<GameState, GameId>, IGameStateRepository {
        private readonly IUnitOfWork _unitOfWork;

        public InMemoryGameStateRepository(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        protected override IList<GameState> GetBaseQuery() {
            return GameDatabase.Get.GameStatesTable.Select(g => g.ToDomainModel()).ToList();
        }

        public override void Save(GameState entity) {
            var dbGameState = DbGameState.FromDomainModel(entity);
            _unitOfWork.Store(dbGameState, tran => GameDatabase.Temp(tran).GameStatesTable);
        }
    }
}
