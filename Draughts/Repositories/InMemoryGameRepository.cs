using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.GameAggregate.Specifications;
using Draughts.Repositories.Database;
using Draughts.Repositories.Databases;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories {
    public class InMemoryGameRepository : InMemoryRepository<Game>, IGameRepository {
        private readonly IPlayerRepository _playerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InMemoryGameRepository(IPlayerRepository playerRepository, IUnitOfWork unitOfWork) {
            _playerRepository = playerRepository;
            _unitOfWork = unitOfWork;
        }

        protected override IList<Game> GetBaseQuery() {
            var players = _playerRepository.List().ToDictionary(p => p.Id.Id);
            return GameDatabase.GamesTable.Select(g => new Game(
                new GameId(g.Id),
                g.PlayerIds.Select(p => players[p]).ToList(),
                GetTurn(g, players),
                new GameSettings(g.BoardSize, GetFirstMoveColor(g), g.FlyingKings, g.MenCaptureBackwards, g.CaptureConstraints),
                GameState.FromStorage(new GameId(g.Id), g.CurrentGameState),
                g.CreatedAt,
                g.StartedAt,
                g.FinishedAt
            )).ToList();
        }

        private Color GetFirstMoveColor(InMemoryGame g) => g.FirstMoveColorIsWhite ? Color.White : Color.Black;
        private Turn? GetTurn(InMemoryGame g, Dictionary<long, Player> players) {
            return g.TurnPlayerId is null || g.TurnCreatedAt is null || g.TurnExpiresAt is null
                ? null
                : new Turn(players[g.TurnPlayerId.Value], g.TurnCreatedAt.Value, g.TurnExpiresAt.Value - g.TurnCreatedAt.Value);
        }

        public Game FindById(GameId id) => Find(new GameIdSpecification(id));
        public Game? FindByIdOrNull(GameId id) => FindOrNull(new GameIdSpecification(id));

        public override void Save(Game entity) {
            var game = new InMemoryGame {
                Id = entity.Id,
                BoardSize = entity.Settings.BoardSize,
                FirstMoveColorIsWhite = entity.Settings.FirstMove == Color.White,
                FlyingKings = entity.Settings.FlyingKings,
                MenCaptureBackwards = entity.Settings.MenCaptureBackwards,
                CaptureConstraints = entity.Settings.CaptureConstraints,
                CurrentGameState = entity.GameState.ToStorage(),
                CreatedAt = entity.CreatedAt,
                StartedAt = entity.StartedAt,
                FinishedAt = entity.FinishedAt,
                TurnPlayerId = entity.Turn?.Player.Id.Id,
                TurnCreatedAt = entity.Turn?.CreatedAt,
                TurnExpiresAt = entity.Turn?.ExpiresAt,
                PlayerIds = entity.Players.Select(p => p.Id.Id).ToArray(),
            };

            _unitOfWork.Store(game, GameDatabase.TempGamesTable);
        }
    }
}
