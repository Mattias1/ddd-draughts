using Draughts.Common.OoConcepts;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.GameAggregate.Specifications;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories.Transaction;
using SqlQueryBuilder.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using static Draughts.Repositories.Database.JoinEnum;

namespace Draughts.Repositories.Database {
    public class DbGameRepository : DbRepository<Game, DbGame>, IGameRepository {
        private readonly IUnitOfWork _unitOfWork;

        public DbGameRepository(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public Game FindById(GameId id) => Find(new GameIdSpecification(id));
        public Game? FindByIdOrNull(GameId id) => FindOrNull(new GameIdSpecification(id));

        protected override string TableName => "game";
        protected override IInitialQueryBuilder GetBaseQuery() => _unitOfWork.Query(TransactionDomain.Game);
        private IQueryBuilder GetPlayerQuery() => GetBaseQuery().SelectAllFrom("player");

        protected override IQueryBuilder ApplySpec(Specification<Game> spec, IQueryBuilder builder) {
            var joins = spec.RequiredJoins().ToArray();
            if (joins.Contains(PossibleJoins.Player)) {
                builder.Join("player", "game.id", "player.game_id");
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
                result.Add(new Game(
                    new GameId(g.Id),
                    players,
                    GetTurn(g, players),
                    GetGameSettings(g),
                    players.SingleOrDefault(p => p.UserId == g.Victor),
                    GameState.FromStorage(new GameId(g.Id), g.CurrentGameState, g.CaptureSequenceFrom),
                    g.CreatedAt,
                    g.StartedAt,
                    g.FinishedAt
                ));
            }
            return result.AsReadOnly();
        }

        protected override Game Parse(DbGame g) {
            var players = GetPlayerQuery().Where("game_id").Is(g.Id).List<DbPlayer>()
                .Select(ParsePlayer).ToList();
            return new Game(
                new GameId(g.Id),
                players,
                GetTurn(g, players),
                GetGameSettings(g),
                players.SingleOrDefault(p => p.UserId == g.Victor),
                GameState.FromStorage(new GameId(g.Id), g.CurrentGameState, g.CaptureSequenceFrom),
                g.CreatedAt,
                g.StartedAt,
                g.FinishedAt
            );
        }

        private Player ParsePlayer(DbPlayer q) {
            return new Player(
                new PlayerId(q.Id),
                new UserId(q.UserId),
                new Username(q.Username),
                Rank.Ranks.All.Single(r => r.Name == q.Rank),
                q.Color ? Color.White : Color.Black,
                q.CreatedAt
            );
        }

        private Turn? GetTurn(DbGame g, List<Player> players) {
            return g.TurnPlayerId is null || g.TurnCreatedAt is null || g.TurnExpiresAt is null ? null : new Turn(
                players.Single(p => p.Id == g.TurnPlayerId.Value),
                g.TurnCreatedAt.Value,
                g.TurnExpiresAt.Value - g.TurnCreatedAt.Value
            );
        }

        private GameSettings GetGameSettings(DbGame g) {
            Color firstMoveColor = g.FirstMoveColorIsWhite ? Color.White : Color.Black;
            var capConstraints = g.CaptureConstraints switch
            {
                "max" => GameSettings.DraughtsCaptureConstraints.MaximumPieces,
                "seq" => GameSettings.DraughtsCaptureConstraints.AnyFinishedSequence,
                _ => throw new InvalidOperationException("Unknown capture constraint.")
            };
            return new GameSettings(g.BoardSize, firstMoveColor, g.FlyingKings, g.MenCaptureBackwards, capConstraints);
        }

        public override void Save(Game entity) {
            var obj = DbGame.FromDomainModel(entity);
            if (FindByIdOrNull(entity.Id) is null) {
                GetBaseQuery().InsertInto("game").InsertFrom(obj).Execute();
            }
            else {
                GetBaseQuery().Update("game").SetWithoutIdFrom(obj).Where("id").Is(entity.Id).Execute();
            }
        }
    }
}
