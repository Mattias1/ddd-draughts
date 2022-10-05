using Draughts.Common.OoConcepts;
using Draughts.Domain.GameContext.Models;
using Draughts.Repositories.Misc;
using Draughts.Repositories.Transaction;
using NodaTime;
using SqlQueryBuilder.Builder;
using SqlQueryBuilder.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using static Draughts.Repositories.Misc.JoinEnum;

namespace Draughts.Repositories;

public sealed class GameRepository : BaseRepository<Game, GameId, DbGame> {
    public GameRepository(IRepositoryUnitOfWork unitOfWork) : base(unitOfWork) { }

    public IReadOnlyList<GameId> ListGameIdsForExpiredTurns(ZonedDateTime datetime) {
        return GetBaseQuery()
            .Select("id").From(TableName)
            .Where("finished_at").IsNull() // For optimization purposes
            .Where("turn_expires_at").Lt(datetime)
            .ListLongs()
            .MapReadOnly(l => new GameId(l));
    }

    protected override string TableName => "games";
    private const string PlayersTableName = "players";
    protected override IInitialQueryBuilder GetBaseQuery() => UnitOfWork.Query(TransactionDomain.Game);
    private IQueryBuilder GetPlayerQuery() => GetBaseQuery().SelectAllFrom(PlayersTableName);

    protected override IQueryBuilder ApplySpec(Specification<Game> spec, IQueryBuilder builder) {
        var joins = spec.RequiredJoins().ToArray();
        if (joins.Contains(PossibleJoins.Player)) {
            builder.Join(PlayersTableName, "game.id", PlayersTableName + ".game_id");
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

    protected override void SaveInternal(Game entity) {
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
            .Select("id").From(PlayersTableName).Where("game_id").Is(gameEntity.Id)
            .ListLongs();
        foreach (var playerEntity in gameEntity.Players) {
            var obj = DbPlayer.FromDomainModel(playerEntity, gameEntity.Id);
            if (existingPlayerIds.Contains(playerEntity.Id.Value)) {
                GetBaseQuery().Update(PlayersTableName).SetWithoutIdFrom(obj).Where("id").Is(playerEntity.Id).Execute();
            }
            else {
                GetBaseQuery().InsertInto(PlayersTableName).InsertFrom(obj).Execute();
            }
        }
    }
}
