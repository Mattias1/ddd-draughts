using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.GameAggregate.Specifications;
using Draughts.Repositories.Transaction;
using SqlQueryBuilder.Builder;
using System;

namespace Draughts.Repositories.Database {
    public class DbPlayerRepository : DbRepository<Player, PlayerId, DbPlayer>, IPlayerRepository {
        private readonly IUnitOfWork _unitOfWork;

        public DbPlayerRepository(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        protected override string TableName => "player";
        protected override IInitialQueryBuilder GetBaseQuery() => _unitOfWork.Query(TransactionDomain.Game);

        protected override Player Parse(DbPlayer q) => q.ToDomainModel();

        public override void Save(Player entity) {
            //  Note: the real fix is to remove this repository, as it should be part of the game aggregate's save operation
            throw new InvalidOperationException("Can't use this method, as we need a gameId to save a player :(");
        }

        public void Save(Player entity, GameId gameId) {
            var obj = DbPlayer.FromDomainModel(entity, gameId);
            if (FindByIdOrNull(entity.Id) is null) {
                GetBaseQuery().InsertInto("player").InsertFrom(obj).Execute();
            }
            else {
                GetBaseQuery().Update("player").SetWithoutIdFrom(obj).Where("id").Is(entity.Id).Execute();
            }
        }
    }
}
