using Draughts.Common;
using Draughts.Common.Utilities;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using NodaTime.Testing;
using System;
using System.Linq;
using Xunit;

namespace Draughts.Test.Domain.GameAggregate {
    public class PlayGameTest {
        private FakeClock _fakeClock = FakeClock.FromUtc(2020, 02, 29);

        [Fact]
        public void DoMoveAndSwitchTurn() {
            // |_|4|_|4|_|4|
            // |4|_|4|_|4|_|
            // |_|.|_|.|_|.|
            // |.|_|.|_|.|_|
            // |_|5|_|5|_|5|
            // |5|_|5|_|5|_|
            var game = GameTestHelper.StartedMiniGame().Build();

            var result = DoMove(game, 13, 11);

            result.Should().Be(GameState.MoveResult.NextTurn);
            game.GameState.Board.ToLongString(" ", "").Should().Be("444 444 000 050 055 555");
            game.GameState.CaptureSequenceFrom.Should().BeNull();
            game.Turn!.Player.Color.Should().Be(Color.Black);
        }

        [Fact]
        public void DoMoveChainCaptureDoesntSwitchTurn() {
            // |_|4|_|4|_|.|
            // |4|_|4|_|.|_|
            // |_|.|_|.|_|4|
            // |5|_|5|_|5|_|
            // |_|.|_|.|_|5|
            // |.|_|.|_|5|_|
            var game = GameBuilder("440 440 004 555 005 005", Color.Black).Build();

            var result = DoMove(game, 9, 14);

            result.Should().Be(GameState.MoveResult.MoreCapturesAvailable);
            game.GameState.Board.ToLongString(" ", "").Should().Be("440 440 000 550 045 005");
            game.GameState.CaptureSequenceFrom.Should().NotBeNull();
            game.GameState.CaptureSequenceFrom!.Should().Be(new SquareId(14));
            game.Turn!.Player.Color.Should().Be(Color.Black);
        }

        [Fact]
        public void DoMoveSwitchTurnAfterLastChainCapture() {
            // |_|4|_|4|_|.|
            // |4|_|4|_|.|_|
            // |_|.|_|.|_|.|
            // |5|_|5|_|.|_|
            // |_|.|_|4|_|5|
            // |.|_|.|_|5|_|
            var game = GameBuilder("440 440 000 550 045 005", Color.Black, 14).Build();

            var result = DoMove(game, 14, 7);

            result.Should().Be(GameState.MoveResult.NextTurn);
            game.GameState.Board.ToLongString(" ", "").Should().Be("440 440 400 500 005 005");
            game.GameState.CaptureSequenceFrom.Should().BeNull();
            game.Turn!.Player.Color.Should().Be(Color.White);
        }

        [Fact]
        public void DoMoveAndPromote() {
            // |_|4|_|4|_|.|
            // |4|_|4|_|.|_|
            // |_|.|_|.|_|5|
            // |5|_|.|_|.|_|
            // |_|4|_|5|_|.|
            // |.|_|.|_|.|_|
            var game = GameBuilder("440 440 005 500 450 000", Color.Black).Build();

            var result = DoMove(game, 13, 16);

            result.Should().Be(GameState.MoveResult.NextTurn);
            game.GameState.Board.ToLongString(" ", "").Should().Be("440 440 005 500 050 600");
            game.GameState.CaptureSequenceFrom.Should().BeNull();
            game.Turn!.Player.Color.Should().Be(Color.White);
        }

        [Fact]
        public void DoMoveAndFinishTheGame() {
            // |_|4|_|.|_|.|
            // |4|_|.|_|.|_|
            // |_|.|_|.|_|4|
            // |4|_|5|_|.|_|
            // |_|.|_|.|_|.|
            // |6|_|.|_|.|_|
            var game = GameBuilder("400 400 004 450 000 600", Color.Black).Build();

            var result = DoMove(game, 16, 3);

            result.Should().Be(GameState.MoveResult.GameOver);
            game.GameState.Board.ToLongString(" ", "").Should().Be("406 400 004 400 000 000");
            game.GameState.CaptureSequenceFrom.Should().BeNull();
            game.Turn.Should().BeNull();
        }

        [Fact]
        public void CantMoveWhenGameHasntStarted() {
            var whitePlayer = PlayerTestHelper.White().Build();
            var game = GameTestHelper.PendingMiniGame(whitePlayer).Build();

            Action doMove = () => DoMoveAs(game, 13, 11, Color.White);

            doMove.Should().Throw<ManualValidationException>().WithMessage(Game.ERROR_GAME_NOT_ACTIVE);
        }

        [Fact]
        public void CantMoveWhenGameIsFinished() {
            var game = GameTestHelper.FinishedMiniGame(Color.White).Build();

            Action doMove = () => DoMoveAs(game, 9, 6, Color.White);

            doMove.Should().Throw<ManualValidationException>().WithMessage(Game.ERROR_GAME_NOT_ACTIVE);
        }

        [Fact]
        public void CantMoveWhenNotYourTurn() {
            var game = GameTestHelper.StartedMiniGame().WithTurn(Color.White).Build();

            Action doMove = () => DoMoveAs(game, 4, 7, Color.Black);

            doMove.Should().Throw<ManualValidationException>().WithMessage(Game.ERROR_NOT_YOUR_TURN);
        }

        [Fact]
        public void CantMoveOpponentsPieces() {
            var game = GameTestHelper.StartedMiniGame().WithTurn(Color.White).Build();

            Action doMove = () => DoMove(game, 4, 7);

            doMove.Should().Throw<ManualValidationException>($"You can only move {Color.White} pieces.");
        }

        [Fact]
        public void CantMoveOutsideTheBoard() {
            var game = GameTestHelper.StartedMiniGame().Build();

            Action toOutside = () => DoMove(game, 18, 20);
            Action fromOutside = () => DoMove(game, 20, 18);

            toOutside.Should().Throw<ManualValidationException>(GameState.ERROR_INVALID_SQUARES);
            fromOutside.Should().Throw<ManualValidationException>(GameState.ERROR_INVALID_SQUARES);
        }

        [Fact]
        public void CantMoveWrongPieceInChainCapture() {
            // |_|4|_|4|_|.|
            // |4|_|4|_|.|_|
            // |_|.|_|.|_|.|
            // |5|_|5|_|.|_|
            // |_|.|_|4|_|5|
            // |.|_|.|_|5|_|
            var game = GameBuilder("440 440 000 550 045 005", Color.Black, 14).Build();

            Action doMove = () => DoMove(game, 5, 8);

            doMove.Should().Throw<ManualValidationException>().WithMessage(GameState.ERROR_CAPTURE_SEQUENCE);
        }

        private GameTestHelper.GameBuilder GameBuilder(string boardString, Color turn, int? captureSequenceFrom = null) {
            var board = BoardPosition.FromString(boardString);
            return GameTestHelper.StartedMiniGame()
                .WithGameState(boardString, captureSequenceFrom)
                .WithTurn(turn);
        }

        private GameState.MoveResult DoMove(Game game, int from, int to) => DoMoveAs(game, from, to, game.Turn!.Player.Color);
        private GameState.MoveResult DoMoveAs(Game game, int from, int to, Color color) {
            var player = game.Players.Single(p => p.Color == color);
            return game.DoMove(player.UserId, new SquareId(from), new SquareId(to), _fakeClock.UtcNow());
        }
    }
}
