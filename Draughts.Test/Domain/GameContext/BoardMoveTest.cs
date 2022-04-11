using Draughts.Common;
using Draughts.Domain.GameContext.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using System;
using Xunit;

namespace Draughts.Test.Domain.GameContext;

public sealed class BoardMoveTest {
    private readonly GameSettings _settings = GameSettings.International;

    [Fact]
    public void InvalidMoveTrows() {
        // |_|4|_|.|
        // |.|_|.|_|
        // |_|.|_|.|
        // |.|_|5|_|
        var board = Board.FromString("40 00 00 05");
        Action doMove = () => board.PerformNewMove(1.AsSquare(), 5.AsSquare(), _settings, out _);
        doMove.Should().Throw<ManualValidationException>();
        board.ToLongString(" ", "").Should().Be("40 00 00 05");
    }

    [Fact]
    public void MovePiece() {
        // |_|4|_|.|
        // |.|_|.|_|
        // |_|.|_|.|
        // |.|_|5|_|
        var board = Board.FromString("40 00 00 05");
        board.PerformNewMove(1.AsSquare(), 3.AsSquare(), _settings, out bool canCaptureMore);
        board.ToLongString(" ", "").Should().Be("00 40 00 05");
        canCaptureMore.Should().BeFalse();
    }

    [Fact]
    public void CapturePiece() {
        // |_|4|_|.|
        // |.|_|5|_|
        // |_|.|_|.|
        // |.|_|5|_|
        var board = Board.FromString("40 05 00 05");
        board.PerformNewMove(1.AsSquare(), 6.AsSquare(), _settings, out bool canCaptureMore);
        board.ToLongString(" ", "").Should().Be("00 00 04 05");
        canCaptureMore.Should().BeFalse();
    }

    [Fact]
    public void InValidMoveInChainSequenceThrows() {
        // |_|4|_|.|
        // |.|_|5|_|
        // |_|4|_|.|
        // |.|_|5|_|
        var board = Board.FromString("40 05 40 05");
        Action doMove = () => board.PerformChainCaptureMove(1.AsSquare(), 2.AsSquare(), _settings, out bool _);
        doMove.Should().Throw<ManualValidationException>();
        board.ToLongString(" ", "").Should().Be("40 05 40 05");
    }

    [Fact]
    public void ValidNonCaptureMoveInChainSequenceThrows() {
        // |_|4|_|.|
        // |.|_|5|_|
        // |_|4|_|.|
        // |.|_|5|_|
        var board = Board.FromString("40 05 40 05");
        Action doMove = () => board.PerformChainCaptureMove(1.AsSquare(), 3.AsSquare(), _settings, out bool _);
        doMove.Should().Throw<ManualValidationException>();
        board.ToLongString(" ", "").Should().Be("40 05 40 05");
    }

    [Fact]
    public void CapturePieceInChainSequence() {
        // |_|4|_|.|
        // |.|_|5|_|
        // |_|4|_|.|
        // |.|_|5|_|
        var board = Board.FromString("40 05 40 05");
        board.PerformChainCaptureMove(1.AsSquare(), 6.AsSquare(), _settings, out bool canCaptureMore);
        board.ToLongString(" ", "").Should().Be("00 00 44 05");
        canCaptureMore.Should().BeFalse();
    }

    [Fact]
    public void BlackManPromotesOnLastLine() {
        // |_|.|_|.|
        // |.|_|.|_|
        // |_|4|_|.|
        // |.|_|.|_|
        var board = Board.FromString("00 00 40 00");
        board.PerformNewMove(5.AsSquare(), 7.AsSquare(), _settings, out _);
        board.ToLongString(" ", "").Should().Be("00 00 00 60");
    }

    [Fact]
    public void WhiteManPromotesOnFirstLine() {
        // |_|.|_|.|
        // |.|_|5|_|
        // |_|.|_|.|
        // |.|_|.|_|
        var board = Board.FromString("00 05 00 00");
        board.PerformNewMove(4.AsSquare(), 2.AsSquare(), _settings, out _);
        board.ToLongString(" ", "").Should().Be("07 00 00 00");
    }

    [Fact]
    public void BlackManDoesntPromoteOnFirstLine() {
        // |_|.|_|.|
        // |.|_|5|_|
        // |_|.|_|4|
        // |.|_|.|_|
        var board = Board.FromString("00 05 04 00");
        board.PerformNewMove(6.AsSquare(), 1.AsSquare(), _settings, out _);
        board.ToLongString(" ", "").Should().Be("40 00 00 00");
    }

    [Fact]
    public void KingStaysAsHeIs() {
        // |_|.|_|.|
        // |.|_|.|_|
        // |_|6|_|.|
        // |.|_|.|_|
        var board = Board.FromString("00 00 60 00");
        board.PerformNewMove(5.AsSquare(), 7.AsSquare(), _settings, out _);
        board.ToLongString(" ", "").Should().Be("00 00 00 60");
    }

    [Fact]
    public void DontPromoteInsideChainCapture() {
        // |_|.|_|.|_|.|
        // |.|_|4|_|4|_|
        // |_|5|_|.|_|.|
        // |.|_|.|_|.|_|
        // |_|.|_|.|_|.|
        // |.|_|.|_|.|_|
        var board = Board.FromString("000 044 500 000 000 000");
        board.PerformNewMove(7.AsSquare(), 2.AsSquare(), _settings, out bool canCaptureMore);
        board.ToLongString(" ", "").Should().Be("050 004 000 000 000 000");
        canCaptureMore.Should().BeTrue();
    }
}
