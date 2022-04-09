using Draughts.Domain.GameContext.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using System.Linq;
using Xunit;

namespace Draughts.Test.Domain.GameContext;

public sealed class LoadGameStateTest {
    [Fact]
    public void LoadGameFromMoves() {
        // |_|4|_|4|_|4|
        // |4|_|4|_|4|_|
        // |_|.|_|.|_|.|
        // |.|_|.|_|.|_|
        // |_|5|_|5|_|5|
        // |5|_|5|_|5|_|
        string moves = "4-7, 14-11, 7x14, 18x11,";

        // |_|4|_|4|_|4|
        // |.|_|4|_|4|_|
        // |_|.|_|.|_|.|
        // |.|_|5|_|.|_|
        // |_|5|_|.|_|5|
        // |5|_|5|_|.|_|
        moves += "1-4, 17-14, 6-9, 11-7,";

        // |_|.|_|4|_|4|
        // |4|_|4|_|.|_|
        // |_|5|_|.|_|4|
        // |.|_|.|_|.|_|
        // |_|5|_|5|_|5|
        // |5|_|.|_|.|_|
        moves += "5x10, 10x17, 17x12, 15x8";

        // |_|.|_|4|_|4|
        // |4|_|.|_|.|_|
        // |_|.|_|5|_|4|
        // |.|_|.|_|.|_|
        // |_|.|_|.|_|.|
        // |5|_|.|_|.|_|
        var game = GameTestHelper.StartedMiniGame().Build();
        var parsedMoves = moves.Split(',').Select(Move.FromString);
        var gameState = GameState.FromStorage(game.Id, game.Settings, null, parsedMoves);
        gameState.Board.ToLongString(" ", "").Should().Be("044 400 054 000 000 500");
        gameState.CaptureSequenceFrom.Should().BeNull();
    }

    [Fact]
    public void LoadGameFromMovesWithCapturesLeft() {
        // |_|.|_|4|_|4|
        // |4|_|.|_|.|_|
        // |_|x|_|.|_|4|
        // |.|_|.|_|.|_|
        // |_|x|_|5|_|5|
        // |5|_|4|_|.|_|
        string moves = "4-7, 14-11, 7x14, 18x11,";
        moves += "1-4, 17-14, 6-9, 11-7,";
        moves += "5x10, 10x17";

        var game = GameTestHelper.StartedMiniGame().Build();
        var parsedMoves = moves.Split(',').Select(Move.FromString);
        var gameState = GameState.FromStorage(game.Id, game.Settings, null, parsedMoves);

        gameState.Board.ToLongString(" ", "").Should().Be("044 400 004 000 055 540");
        gameState.CaptureSequenceFrom.Should().Be(17.AsSquare());
    }
}
