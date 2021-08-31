using Draughts.Common.OoConcepts;
using Draughts.Domain.GameContext.Models;
using Draughts.Repositories.Transaction;
using SqlQueryBuilder.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using static Draughts.Repositories.Database.JoinEnum;

namespace Draughts.Repositories.Database {
    public class DbGameRepository : DbRepository<Game, GameId, DbGame>, IGameRepository {
        private readonly IUnitOfWork _unitOfWork;

        public DbGameRepository(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        protected override string TableName => "game";
        private const string PlayerTableName = "player";
        protected override IInitialQueryBuilder GetBaseQuery() => _unitOfWork.Query(TransactionDomain.Game);
        private IQueryBuilder GetPlayerQuery() => GetBaseQuery().SelectAllFrom(PlayerTableName);

        protected override IQueryBuilder ApplySpec(Specification<Game> spec, IQueryBuilder builder) {
            var joins = spec.RequiredJoins().ToArray();
            if (joins.Contains(PossibleJoins.Player)) {
                builder.Join(PlayerTableName, "game.id", PlayerTableName + ".game_id");
            }
            return base.ApplySpec(spec, builder);
        }

        protected override IReadOnlyList<Game> Parse(IReadOnlyList<DbGame> gs) {
            if (gs.Count == 0) {
                return new List<Game>().AsReadOnly();
            }

            var allPlayers = GetPlayerQuery().Where("game_id").In(gs.Select(g => g.Id)).List<DbPlayer>()
                .ToLookup(p => p.GameId, ParsePlayer);
            var result = new List<Game>(gs.Count);
            foreach (var g in gs) {
                var players = allPlayers[g.Id].ToList();
                result.Add(g.ToDomainModel(players));
            }
            return result.AsReadOnly();
        }

        protected override Game Parse(DbGame g) {
            var players = GetPlayerQuery().Where("game_id").Is(g.Id).List<DbPlayer>()
                .Select(ParsePlayer).ToList();
            return g.ToDomainModel(players);
        }

        private Player ParsePlayer(DbPlayer q) => q.ToDomainModel();

        public override void Save(Game entity) {
            var obj = DbGame.FromDomainModel(entity);
            if (FindByIdOrNull(entity.Id) is null) {
                GetBaseQuery().InsertInto(TableName).InsertFrom(obj).Execute();
            }
            else {
                GetBaseQuery().Update(TableName).SetWithoutIdFrom(obj).Where("id").Is(entity.Id).Execute();
            }
            SavePlayers(entity);
        }

        private void SavePlayers(Game gameEntity) {
            var existingPlayerIds = GetBaseQuery()
                .Select("id").From(PlayerTableName).Where("game_id").Is(gameEntity.Id)
                .ListLongs();
            foreach (var playerEntity in gameEntity.Players) {
                var obj = DbPlayer.FromDomainModel(playerEntity, gameEntity.Id);
                if (existingPlayerIds.Contains(playerEntity.Id.Value)) {
                    GetBaseQuery().Update(PlayerTableName).SetWithoutIdFrom(obj).Where("id").Is(playerEntity.Id).Execute();
                }
                else {
                    GetBaseQuery().InsertInto(PlayerTableName).InsertFrom(obj).Execute();
                }
            }
        }
    }
}
