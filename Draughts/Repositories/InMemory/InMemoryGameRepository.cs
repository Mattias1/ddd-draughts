using Draughts.Domain.GameAggregate.Models;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.InMemory {
    public class InMemoryGameRepository : InMemoryRepository<Game, GameId>, IGameRepository {
        private readonly IUnitOfWork _unitOfWork;

        public InMemoryGameRepository(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        protected override IList<Game> GetBaseQuery() {
            var players = GameDatabase.Get.PlayersTable.ToLookup(p => p.GameId, p => p.ToDomainModel());
            return GameDatabase.Get.GamesTable
                .Select(g => g.ToDomainModel(players[g.Id].ToList()))
                .ToList();
        }

        public override void Save(Game entity) {
            var game = DbGame.FromDomainModel(entity);
            _unitOfWork.Store(game, tran => GameDatabase.Temp(tran).GamesTable);
        }
    }
}
