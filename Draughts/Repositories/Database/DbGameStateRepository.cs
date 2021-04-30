using Draughts.Domain.GameAggregate.Models;
using Draughts.Repositories.Transaction;
using SqlQueryBuilder.Builder;

namespace Draughts.Repositories.Database {
    public class DbGameStateRepository : DbRepository<GameState, GameId, DbGameState>, IGameStateRepository {
        private readonly IUnitOfWork _unitOfWork;

        public DbGameStateRepository(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        protected override string TableName => "gamestate";
        protected override IInitialQueryBuilder GetBaseQuery() => _unitOfWork.Query(TransactionDomain.Game);

        protected override GameState Parse(DbGameState gs) => gs.ToDomainModel();

        public override void Save(GameState entity) {
            var obj = DbGameState.FromDomainModel(entity);
            if (FindByIdOrNull(entity.Id) is null) {
                GetBaseQuery().InsertInto(TableName).InsertFrom(obj).Execute();
            }
            else {
                GetBaseQuery().Update(TableName).SetWithoutIdFrom(obj).Where("id").Is(entity.Id).Execute();
            }
        }
    }
}
