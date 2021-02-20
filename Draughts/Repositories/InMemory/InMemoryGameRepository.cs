using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.GameAggregate.Specifications;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.InMemory {
    public class InMemoryGameRepository : InMemoryRepository<Game>, IGameRepository {
        private readonly IUnitOfWork _unitOfWork;

        public InMemoryGameRepository(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        protected override IList<Game> GetBaseQuery() {
            var players = GameDatabase.PlayersTable.ToLookup(p => p.GameId, p => p.ToDomainModel());
            return GameDatabase.GamesTable
                .Select(g => g.ToDomainModel(players[g.Id].ToList()))
                .ToList();
        }

        public Game FindById(GameId id) => Find(new GameIdSpecification(id));
        public Game? FindByIdOrNull(GameId id) => FindOrNull(new GameIdSpecification(id));

        public override void Save(Game entity) {
            var game = DbGame.FromDomainModel(entity);
            _unitOfWork.Store(game, GameDatabase.TempGamesTable);
        }
    }
}
