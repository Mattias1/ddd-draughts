using Draughts.Common.Utilities;
using Draughts.Domain.GameAggregate.Models;
using NodaTime;
using NodaTime.Testing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Test.TestHelpers {
    public class GameTestHelper {
        private static readonly ZonedDateTime Feb29 = FakeClock.FromUtc(2020, 02, 29).UtcNow();

        public static GameBuilder FinishedMiniGame(Color victor) {
            char c = victor == Color.Black ? '4' : '5';
            return StartedMiniGame()
                .WithGameState($"000 000 00{c} 000 000 000")
                .WithFinishedAt(Feb29)
                .WithTurn((Turn?)null)
                .WithVictor(victor);
        }

        public static GameBuilder StartedMiniGame() {
            var whitePlayer = PlayerTestHelper.White().Build();
            var blackPlayer = PlayerTestHelper.Black().Build();
            var turn = new Turn(whitePlayer, Feb29, Duration.FromHours(24));

            return PendingMiniGame()
                .WithPlayers(whitePlayer, blackPlayer)
                .WithTurn(turn)
                .WithStartedAt(Feb29);
        }

        public static GameBuilder PendingMiniGame(Player creator) => PendingMiniGame().WithPlayers(creator);
        public static GameBuilder PendingMiniGame() => PendingGame(GameSettings.Mini);

        public static GameBuilder StartedInternationalGame() {
            var whitePlayer = PlayerTestHelper.White().Build();
            var blackPlayer = PlayerTestHelper.Black().Build();
            var turn = new Turn(whitePlayer, Feb29, Duration.FromHours(24));

            return PendingInternationalGame()
                .WithPlayers(whitePlayer, blackPlayer)
                .WithTurn(turn)
                .WithStartedAt(Feb29);
        }

        public static GameBuilder PendingInternationalGame(Player creator) => PendingInternationalGame().WithPlayers(creator);
        public static GameBuilder PendingInternationalGame() => PendingGame(GameSettings.International);

        public static GameBuilder PendingGame(GameSettings settings, Player creator) => PendingGame(settings).WithPlayers(creator);
        public static GameBuilder PendingGame(GameSettings settings) {
            var gameId = new GameId(IdTestHelper.NextForGame());
            return new GameBuilder()
                .WithId(gameId)
                .WithSettings(settings)
                .WithGameState(GameState.InitialState(gameId, settings.BoardSize))
                .WithCreatedAt(Feb29);
        }


        public class GameBuilder {
            private GameId? _id;
            private List<Player> _players = new List<Player>(2);
            private Turn? _turn;
            private GameSettings? _settings;
            private Player? _victor;
            private GameState? _gameState;
            private ZonedDateTime? _createdAt;
            private ZonedDateTime? _startedAt;
            private ZonedDateTime? _finishedAt;

            public GameBuilder WithId(long id) => WithId(new GameId(id));
            public GameBuilder WithId(GameId id) {
                _id = id;
                return this;
            }

            public GameBuilder WithPlayers(params Player[] players) => WithPlayers(players.ToList());
            public GameBuilder WithPlayers(List<Player> players) {
                _players = players;
                return this;
            }

            public GameBuilder WithTurn(Color color) {
                var player = FindPlayerForColor(color);
                var createdAt = _startedAt ?? _createdAt ?? Feb29;
                return WithTurn(new Turn(player, createdAt, Duration.FromHours(24)));
            }
            public GameBuilder WithTurn(Turn? turn) {
                _turn = turn;
                return this;
            }

            public GameBuilder WithSettings(GameSettings settings) {
                _settings = settings;
                return this;
            }

            public GameBuilder WithVictor(Color color) {
                return WithVictor(FindPlayerForColor(color));
            }
            public GameBuilder WithVictor(Player? victor) {
                _victor = victor;
                return this;
            }

            public GameBuilder WithGameState(string board, int? captureSequenceFrom = null) {
                if (_id is null) {
                    throw new InvalidOperationException("Game id is null");
                }
                return WithGameState(GameState.FromStorage(_id, board, captureSequenceFrom));
            }
            public GameBuilder WithGameState(GameState gameState) {
                _gameState = gameState;
                return this;
            }

            public GameBuilder WithCreatedAt(ZonedDateTime createdAt) {
                _createdAt = createdAt;
                return this;
            }

            public GameBuilder WithStartedAt(ZonedDateTime? startedAt) {
                _startedAt = startedAt;
                return this;
            }

            public GameBuilder WithFinishedAt(ZonedDateTime? finishedAt) {
                _finishedAt = finishedAt;
                return this;
            }

            private Player FindPlayerForColor(Color color) {
                var player = _players.SingleOrDefault(p => p.Color == color);
                if (player is null) {
                    throw new InvalidOperationException("No player available with this color");
                }
                return player;
            }

            public Game Build() {
                if (_id is null) {
                    throw new InvalidOperationException("Id is not nullable");
                }
                if (_settings is null) {
                    throw new InvalidOperationException("Settings is not nullable");
                }
                if (_gameState is null) {
                    throw new InvalidOperationException("GameState is not nullable");
                }
                if (_createdAt is null) {
                    throw new InvalidOperationException("CreatedAt is not nullable");
                }

                return new Game(_id, _players, _turn, _settings, _victor, _gameState, _createdAt.Value, _startedAt, _finishedAt);
            }
        }
    }
}
