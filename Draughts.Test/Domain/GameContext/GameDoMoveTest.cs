using Draughts.Common;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.GameContext.Services;
using Draughts.Domain.UserContext.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using NodaTime.Testing;
using System;
using System.Linq;
using Xunit;

namespace Draughts.Test.Domain.GameContext;

public sealed class GameDoMoveTest {
    private const string INITIAL_BOARD = "444 444 000 000 555 555";
    private readonly PlayGameDomainService _playGameService;

    public GameDoMoveTest() {
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
        var (game, gameState, black, white) = BuildGame(INITIAL_BOARD, Color.White);

        _playGameService.DoMove(game, gameState, white, 13.AsSquare(), 11.AsSquare());

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
        var (game, gameState, black, white) = BuildGame("440 440 004 555 005 005", Color.Black);

        _playGameService.DoMove(game, gameState, black, 9.AsSquare(), 14.AsSquare());

        gameState.Board.ToLongString(" ", "").Should().Be("440 440 000 55D 045 005");
        gameState.CaptureSequenceFrom.Should().NotBeNull();
        gameState.CaptureSequenceFrom!.Should().Be(14.AsSquare());
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
        var (game, gameState, black, white) = BuildGame("440 440 004 555 005 005", Color.Black, "9x14");

        _playGameService.DoMove(game, gameState, black, 14.AsSquare(), 7.AsSquare());

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
        var (game, gameState, black, white) = BuildGame("440 440 005 500 450 000", Color.Black);

        _playGameService.DoMove(game, gameState, black, 13.AsSquare(), 16.AsSquare());

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
        var (game, gameState, black, white) = BuildGame("400 400 004 450 000 600", Color.Black);

        _playGameService.DoMove(game, gameState, black, 16.AsSquare(), 3.AsSquare());

        gameState.Board.ToLongString(" ", "").Should().Be("406 400 004 400 000 000");
        gameState.CaptureSequenceFrom.Should().BeNull();
        game.Turn.Should().BeNull();
        game.Victor?.Color.Should().Be(Color.Black);
    }

    [Fact]
    public void CantMoveWhenGameHasntStarted() {
        var whitePlayer = PlayerTestHelper.White().Build();
        var game = GameTestHelper.PendingMiniGame(whitePlayer).Build();
        var gameState = GameState.InitialState(game.Id, game.Settings.BoardType);

        Action doMove = () => _playGameService.DoMove(game, gameState, whitePlayer.UserId, 13.AsSquare(), 11.AsSquare());

        doMove.Should().Throw<ManualValidationException>().WithMessage("*not active*");
    }

    [Fact]
    public void CantMoveWhenGameIsFinished() {
        var game = GameTestHelper.FinishedMiniGame(Color.White).Build();
        var gameState = GameState.FromStorage(game.Id, game.Settings, "000 000 005 000 000 000", new Move[0]);
        var white = game.Players.Single(p => p.Color == Color.White).UserId;

        Action doMove = () => _playGameService.DoMove(game, gameState, white, 9.AsSquare(), 6.AsSquare());

        doMove.Should().Throw<ManualValidationException>().WithMessage("*not active*");
    }

    [Fact]
    public void CantMoveWhenNotYourTurn() {
        var (game, gameState, black, white) = BuildGame(INITIAL_BOARD, Color.White);

        Action doMove = () => _playGameService.DoMove(game, gameState, black, 4.AsSquare(), 7.AsSquare());

        doMove.Should().Throw<ManualValidationException>().WithMessage("*not your turn*");
    }

    [Fact]
    public void CantMoveOpponentsPieces() {
        var (game, gameState, black, white) = BuildGame(INITIAL_BOARD, Color.White);

        Action doMove = () => _playGameService.DoMove(game, gameState, black, 4.AsSquare(), 7.AsSquare());

        doMove.Should().Throw<ManualValidationException>($"You can only move {Color.White} pieces.");
    }

    [Fact]
    public void CantMoveOutsideTheBoard() {
        var (game, gameState, black, white) = BuildGame(INITIAL_BOARD, Color.White);

        Action toOutside = () => _playGameService.DoMove(game, gameState, black, 18.AsSquare(), 20.AsSquare());
        Action fromOutside = () => _playGameService.DoMove(game, gameState, black, 20.AsSquare(), 18.AsSquare());

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
        var (game, gameState, black, white) = BuildGame("440 440 004 555 005 005", Color.Black, "9x14");

        Action doMove = () => _playGameService.DoMove(game, gameState, black, 5.AsSquare(), 8.AsSquare());

        doMove.Should().Throw<ManualValidationException>().WithMessage(GameState.ERROR_CAPTURE_SEQUENCE);
    }

    private (Game game, GameState gameState, UserId black, UserId white)
            BuildGame(string boardString, Color turn, params string[] moves) {
        var game = GameTestHelper.StartedMiniGame().WithTurn(turn).Build();
        var gameState = GameState.FromStorage(game.Id, game.Settings, boardString, moves.Select(Move.FromString));
        var blackPlayer = game.Players.Single(p => p.Color == Color.Black);
        var whitePlayer = game.Players.Single(p => p.Color == Color.White);
        return (game, gameState, blackPlayer.UserId, whitePlayer.UserId);
    }
}
