using Draughts.Common;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.GameContext.Services;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using NodaTime.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Draughts.Test.Domain.GameContext {
    public class PlayGameTest {
        private readonly IPlayGameDomainService _playGameService;

        public PlayGameTest() {
            var clock = FakeClock.FromUtc(2020, 02, 29);
            _playGameService = new PlayGameDomainService(clock);
        }

        [Fact]
        public void DoMoveAndSwitchTurn() {
            // |_|4|_|4|_|4|
            // |4|_|4|_|4|_|
            // |_|.|_|.|_|.|
            // |.|_|.|_|.|_|
            // |_|5|_|5|_|5|
            // |5|_|5|_|5|_|
            var game = GameTestHelper.StartedMiniGame().Build();
            var gameState = GameState.InitialState(game.Id, game.Settings.BoardSize);

            DoMove(game, gameState, 13, 11);

            gameState.Board.ToLongString(" ", "").Should().Be("444 444 000 050 055 555");
            gameState.CaptureSequenceFrom.Should().BeNull();
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
            var (game, gameState) = BuildGame("440 440 004 555 005 005", Color.Black);

            DoMove(game, gameState, 9, 14);

            gameState.Board.ToLongString(" ", "").Should().Be("440 440 000 550 045 005");
            gameState.CaptureSequenceFrom.Should().NotBeNull();
            gameState.CaptureSequenceFrom!.Should().Be(new SquareId(14));
            game.Turn!.Player.Color.Should().Be(Color.Black);
        }

        [Fact]
        public void DoMoveSwitchTurnAfterLastChainCapture() {
            // |_|4|_|4|_|.|
            // |4|_|4|_|.|_|
            // |_|.|_|.|_|.|
            // |5|_|5|_|x|_|
            // |_|.|_|4|_|5|
            // |.|_|.|_|5|_|
            var (game, gameState) = BuildGame("440 440 004 555 005 005", Color.Black, "9x14");

            DoMove(game, gameState, 14, 7);

            gameState.Board.ToLongString(" ", "").Should().Be("440 440 400 500 005 005");
            gameState.CaptureSequenceFrom.Should().BeNull();
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
            var (game, gameState) = BuildGame("440 440 005 500 450 000", Color.Black);

            DoMove(game, gameState, 13, 16);

            gameState.Board.ToLongString(" ", "").Should().Be("440 440 005 500 050 600");
            gameState.CaptureSequenceFrom.Should().BeNull();
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
            var (game, gameState) = BuildGame("400 400 004 450 000 600", Color.Black);

            DoMove(game, gameState, 16, 3);

            gameState.Board.ToLongString(" ", "").Should().Be("406 400 004 400 000 000");
            gameState.CaptureSequenceFrom.Should().BeNull();
            game.Turn.Should().BeNull();
            game.Victor?.Color.Should().Be(Color.Black);
        }

        [Fact]
        public void CantMoveWhenGameHasntStarted() {
            var whitePlayer = PlayerTestHelper.White().Build();
            var game = GameTestHelper.PendingMiniGame(whitePlayer).Build();
            var gameState = GameState.InitialState(game.Id, game.Settings.BoardSize);

            Action doMove = () => DoMoveAs(game, gameState, 13, 11, Color.White);

            doMove.Should().Throw<ManualValidationException>().WithMessage(Game.ERROR_GAME_NOT_ACTIVE);
        }

        [Fact]
        public void CantMoveWhenGameIsFinished() {
            var game = GameTestHelper.FinishedMiniGame(Color.White).Build();
            var gameState = GameState.FromStorage(game.Id, game.Settings, "000 000 005 000 000 000", new Move[0]);

            Action doMove = () => DoMoveAs(game, gameState, 9, 6, Color.White);

            doMove.Should().Throw<ManualValidationException>().WithMessage(Game.ERROR_GAME_NOT_ACTIVE);
        }

        [Fact]
        public void CantMoveWhenNotYourTurn() {
            var game = GameTestHelper.StartedMiniGame().WithTurn(Color.White).Build();
            var gameState = GameState.InitialState(game.Id, game.Settings.BoardSize);

            Action doMove = () => DoMoveAs(game, gameState, 4, 7, Color.Black);

            doMove.Should().Throw<ManualValidationException>().WithMessage(Game.ERROR_NOT_YOUR_TURN);
        }

        [Fact]
        public void CantMoveOpponentsPieces() {
            var game = GameTestHelper.StartedMiniGame().WithTurn(Color.White).Build();
            var gameState = GameState.InitialState(game.Id, game.Settings.BoardSize);

            Action doMove = () => DoMove(game, gameState, 4, 7);

            doMove.Should().Throw<ManualValidationException>($"You can only move {Color.White} pieces.");
        }

        [Fact]
        public void CantMoveOutsideTheBoard() {
            var game = GameTestHelper.StartedMiniGame().Build();
            var gameState = GameState.InitialState(game.Id, game.Settings.BoardSize);

            Action toOutside = () => DoMove(game, gameState, 18, 20);
            Action fromOutside = () => DoMove(game, gameState, 20, 18);

            toOutside.Should().Throw<ManualValidationException>(GameState.ERROR_INVALID_SQUARES);
            fromOutside.Should().Throw<ManualValidationException>(GameState.ERROR_INVALID_SQUARES);
        }

        [Fact]
        public void CantMoveWrongPieceInChainCapture() {
            // |_|4|_|4|_|.|
            // |4|_|4|_|.|_|
            // |_|.|_|.|_|.|
            // |5|_|5|_|x|_|
            // |_|.|_|4|_|5|
            // |.|_|.|_|5|_|
            var (game, gameState) = BuildGame("440 440 004 555 005 005", Color.Black, "9x14");

            Action doMove = () => DoMove(game, gameState, 5, 8);

            doMove.Should().Throw<ManualValidationException>().WithMessage(GameState.ERROR_CAPTURE_SEQUENCE);
        }

        private (Game game, GameState gameState) BuildGame(string boardString, Color turn, params string[] moves) {
            var board = Board.FromString(boardString);
            var game = GameTestHelper.StartedMiniGame().WithTurn(turn).Build();
            var gameState = GameState.FromStorage(game.Id, game.Settings, boardString, moves.Select(Move.FromString));
            return (game, gameState);
        }

        private void DoMove(Game game, GameState gameState, int from, int to) {
            DoMoveAs(game, gameState, from, to, game.Turn!.Player.Color);
        }
        private void DoMoveAs(Game game, GameState gameState, int from, int to, Color color) {
            var player = game.Players.Single(p => p.Color == color);
            _playGameService.DoMove(game, gameState, player.UserId, new SquareId(from), new SquareId(to));
        }
    }
}
