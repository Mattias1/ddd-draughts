using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.GameAggregate.Specifications;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories.Transaction;
using SqlQueryBuilder.Builder;
using System;
using System.Linq;

namespace Draughts.Repositories.Database {
    public class DbPlayerRepository : DbRepository<Player, DbPlayer>, IPlayerRepository {
        private readonly IUnitOfWork _unitOfWork;

        public DbPlayerRepository(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public Player FindById(PlayerId id) => Find(new PlayerIdSpecification(id));
        public Player? FindByIdOrNull(PlayerId id) => FindOrNull(new PlayerIdSpecification(id));

        protected override string TableName => "player";
        protected override IInitialQueryBuilder GetBaseQuery() => _unitOfWork.Query(TransactionDomain.Game);

        protected override Player Parse(DbPlayer q) {
            return new Player(
                new PlayerId(q.Id),
                new UserId(q.UserId),
                new Username(q.Username),
                Rank.Ranks.All.Single(r => r.Name == q.Rank),
                q.Color ? Color.White : Color.Black,
                q.CreatedAt
            );
        }

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
