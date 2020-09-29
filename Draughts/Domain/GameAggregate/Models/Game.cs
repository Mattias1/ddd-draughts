using Draughts.Common;
using Draughts.Common.OoConcepts;
using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Domain.GameAggregate.Models {
    public class Game : Entity<Game, GameId> {
        private readonly List<Player> _players;

        public override GameId Id { get; }
        public IReadOnlyList<Player> Players => _players.AsReadOnly();
        public Turn? Turn { get; private set; }
        public GameSettings Settings { get; }
        public ZonedDateTime CreatedAt { get; }
        public ZonedDateTime? StartedAt { get; private set; }
        public ZonedDateTime? FinishedAt { get; private set; }

        public bool HasStarted => StartedAt != null;

        public bool IsFinished => FinishedAt != null;

        public Game(GameId id, GameSettings settings, ZonedDateTime createdAt)
            : this(id, new List<Player>(2), null, settings, createdAt, null, null) { }
        public Game(
            GameId id, List<Player> players, Turn? turn, GameSettings settings,
            ZonedDateTime createdAt, ZonedDateTime? startedAt, ZonedDateTime? finishedAt
        ) {
            Id = id;
            _players = players;
            Turn = turn;
            Settings = settings;
            CreatedAt = createdAt;
            StartedAt = startedAt;
            FinishedAt = finishedAt;
        }

        public void JoinGame(Player player, ZonedDateTime joinedAt) {
            if (HasStarted) {
                throw new ManualValidationException("Cannot join a game that is already started.");
            }
            if (_players.Contains(player)) {
                throw new ManualValidationException("Cannot join a game you have already joined.");
            }
            if (_players.Any(p => p.UserId == player.UserId)) {
                throw new ManualValidationException($"Cannot join a game you have already joined.");
            }
            if (_players.Any(p => p.Color == player.Color)) {
                throw new ManualValidationException($"The color {player.Color} is already taken.");
            }

            _players.Add(player);

            if (_players.Count == 2) {
                StartGame(joinedAt);
            }
        }

        private void StartGame(ZonedDateTime startedAt) {
            StartedAt = startedAt;
            var startingPlayer = GetPlayerForColor(Settings.FirstMove);
            if (_players[0] != startingPlayer) {
                _players[1] = _players[0];
                _players[0] = startingPlayer;
            }
            Turn = new Turn(startingPlayer, startedAt, Settings.MaxTurnLength);
        }

        private Player GetPlayerForColor(Color color) => _players.Single(p => p.Color == color);
    }
}
