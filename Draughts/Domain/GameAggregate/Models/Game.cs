using Draughts.Common;
using Draughts.Common.OoConcepts;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Domain.GameAggregate.Models {
    public class Game : Entity<Game, GameId> {
        private readonly List<Player> _players;

        public override GameId Id { get; }
        public IReadOnlyList<Player> Players => _players.AsReadOnly();
        public Turn? Turn { get; private set; }
        public GameSettings Settings { get; }
        public GameState GameState { get; private set; }
        public ZonedDateTime CreatedAt { get; }
        public ZonedDateTime? StartedAt { get; private set; }
        public ZonedDateTime? FinishedAt { get; private set; }

        public bool HasStarted => StartedAt != null;
        public bool IsFinished => FinishedAt != null;

        public Game(GameId id, GameSettings settings, ZonedDateTime createdAt)
            : this(id, new List<Player>(2), null, settings, GameState.InitialState(id, settings.BoardSize), createdAt, null, null) { }
        public Game(GameId id, List<Player> players, Turn? turn, GameSettings settings, GameState gameState,
                ZonedDateTime createdAt, ZonedDateTime? startedAt, ZonedDateTime? finishedAt) {
            Id = id;
            _players = players;
            Turn = turn;
            Settings = settings;
            GameState = gameState;
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
            SwitchTurn(startedAt);
        }

        private Player GetPlayerForColor(Color color) => _players.Single(p => p.Color == color);

        public GameState.MoveResult DoMove(SquareNumber from, SquareNumber to, ZonedDateTime movedAt) {
            // TODO: Make sure it's the right players turn and so on and so forth.
            var result = GameState.AddMove(from, to);

            switch (result) {
                case GameState.MoveResult.NextTurn:
                    SwitchTurn(movedAt);
                    return result;
                case GameState.MoveResult.MoreCapturesAvailable:
                    return result; // Don't do anything, the user will do the next move.
                case GameState.MoveResult.GameOver:
                    FinishGame(movedAt);
                    return result;
                default:
                    throw new InvalidOperationException("Unknown MoveResult");
            }
        }

        private void SwitchTurn(ZonedDateTime switchedAt) {
            var player = Turn is null ? GetPlayerForColor(Settings.FirstMove) : Players.Single(p => p != Turn!.Player);
            Turn = new Turn(player, switchedAt, Settings.MaxTurnLength);
        }

        private void FinishGame(ZonedDateTime finishedAt) {
            FinishedAt = finishedAt;
            Turn = null;
        }
    }
}
