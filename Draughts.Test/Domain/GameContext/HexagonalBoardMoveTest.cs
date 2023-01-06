using Draughts.Common;
using Draughts.Domain.GameContext.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using System;
using Xunit;

namespace Draughts.Test.Domain.GameContext;

public sealed class HexagonalBoardMoveTest {
    private static readonly IBoardType HEX3 = new HexagonalBoardType(3);

    private readonly GameSettings _settings = GameSettings.MiniHex;

    [Fact]
    public void InvalidMoveTrows() {
        //   |.|.|.|    Side view   |03|07|12|           E
        //  |4|.|.|5|              |02|06|11|16|      N  +  S
        // |4|4|.|5|5|  <- You    |01|05|10|15|19|       W
        //  |4|.|.|5|              |04|09|14|18|
        //   |.|.|.|                |08|13|17|
        var board = Board.FromString(HEX3, "440 4400 00000 0055 055");
        Action doMove = () => board.PerformNewMove(2.AsSquare(), 7.AsSquare(), _settings, out _);
        doMove.Should().Throw<ManualValidationException>();
        board.ToLongString(" ", "").Should().Be("440 4400 00000 0055 055");
    }

    [Fact]
    public void MovePiece() {
        //   |.|.|.|    Side view   |03|07|12|           E
        //  |4|.|.|5|              |02|06|11|16|      N  +  S
        // |.|.|.|.|.|  <- You    |01|05|10|15|19|       W
        //  |.|.|.|.|              |04|09|14|18|
        //   |.|.|.|                |08|13|17|
        var board = Board.FromString(HEX3, "040 0000 00000 0005 000");
        board.PerformNewMove(16.AsSquare(), 15.AsSquare(), _settings, out bool canCaptureMore);
        board.ToLongString(" ", "").Should().Be("040 0000 00000 0050 000");
        canCaptureMore.Should().BeFalse();
    }

    [Theory]
    [InlineData(5, 6, 7), InlineData(5, 10, 15), InlineData(5, 9, 13)]
    public void CapturePieceForwards(int from, int victim, int to) {
        //   |.|.|5|    Side view   |03|07|12|           E
        //  |.|5|.|.|              |02|06|11|16|      N  +  S
        // |.|4|5|.|5|  <- You    |01|05|10|15|19|       W
        //  |.|5|.|.|              |04|09|14|18|
        //   |.|.|5|                |08|13|17|
        var board = Board.FromString(HEX3, "000 0450 05505 0000 505");
        board.PerformNewMove(from.AsSquare(), to.AsSquare(), _settings, out bool canCaptureMore);
        board[from.AsSquare()].Piece.Should().Be(Piece.Empty);
        board[victim.AsSquare()].Piece.Should().Be(Piece.Empty);
        board[to.AsSquare()].Piece.Should().Be(Piece.BlackMan);
        canCaptureMore.Should().BeFalse();
    }

    [Theory]
    [InlineData(15, 11, 7), InlineData(15, 10, 5), InlineData(15, 14, 13)]
    public void CapturePieceBackwards(int from, int victim, int to) {
        //   |5|.|.|    Side view   |03|07|12|           E
        //  |.|.|5|.|              |02|06|11|16|      N  +  S
        // |5|.|5|4|.|  <- You    |01|05|10|15|19|       W
        //  |.|.|5|.|              |04|09|14|18|
        //   |5|.|.|                |08|13|17|
        var board = Board.FromString(HEX3, "505 0000 50550 0540 000");
        board.PerformNewMove(from.AsSquare(), to.AsSquare(), _settings, out bool canCaptureMore);
        board[from.AsSquare()].Piece.Should().Be(Piece.Empty);
        board[victim.AsSquare()].Piece.Should().Be(Piece.Empty);
        board[to.AsSquare()].Piece.Should().Be(Piece.BlackMan);
        canCaptureMore.Should().BeFalse();
    }

    [Fact]
    public void InValidMoveInChainSequenceThrows() {
        //   |.|.|5|    Side view   |03|07|12|           E
        //  |.|5|.|.|              |02|06|11|16|      N  +  S
        // |.|4|.|.|.|  <- You    |01|05|10|15|19|       W
        //  |.|.|.|.|              |04|09|14|18|
        //   |.|.|.|                |08|13|17|
        var board = Board.FromString(HEX3, "000 0450 00005 0000 000");
        Action doMove = () => board.PerformChainCaptureMove(5.AsSquare(), 15.AsSquare(), _settings, out bool _);
        doMove.Should().Throw<ManualValidationException>();
        board.ToLongString(" ", "").Should().Be("000 0450 00005 0000 000");
    }

    [Fact]
    public void ValidNonCaptureMoveInChainSequenceThrows() {
        //   |.|.|5|    Side view   |03|07|12|           E
        //  |.|5|.|.|              |02|06|11|16|      N  +  S
        // |.|4|.|.|.|  <- You    |01|05|10|15|19|       W
        //  |.|.|.|.|              |04|09|14|18|
        //   |.|.|.|                |08|13|17|
        var board = Board.FromString(HEX3, "000 0450 00005 0000 000");
        Action doMove = () => board.PerformChainCaptureMove(5.AsSquare(), 2.AsSquare(), _settings, out bool _);
        doMove.Should().Throw<ManualValidationException>();
        board.ToLongString(" ", "").Should().Be("000 0450 00005 0000 000");
    }

    [Fact]
    public void CapturePieceInChainSequence() {
        //   |.|.|5|    Side view   |03|07|12|           E
        //  |.|5|.|.|              |02|06|11|16|      N  +  S
        // |.|4|.|.|.|  <- You    |01|05|10|15|19|       W
        //  |.|.|.|.|              |04|09|14|18|
        //   |.|.|.|                |08|13|17|
        var board = Board.FromString(HEX3, "000 0450 00005 0000 000");
        board.PerformChainCaptureMove(5.AsSquare(), 7.AsSquare(), _settings, out bool canCaptureMore);
        board.ToLongString(" ", "").Should().Be("000 0004 00005 0000 000");
        canCaptureMore.Should().BeFalse();
    }

    [Theory]
    [InlineData(11, 12), InlineData(11, 16)]
    [InlineData(15, 16), InlineData(15, 19)]
    [InlineData(14, 18), InlineData(14, 17)]
    public void BlackManPromotesOnLastLine(int from, int to) {
        //   |.|.|.|    Side view   |03|07|12|           E
        //  |.|.|4|.|              |02|06|11|16|      N  +  S
        // |.|.|.|4|.|  <- You    |01|05|10|15|19|       W
        //  |.|.|4|.|              |04|09|14|18|
        //   |.|.|.|                |08|13|17|
        var board = Board.FromString(HEX3, "000 0000 00040 0440 000");
        board.PerformNewMove(from.AsSquare(), to.AsSquare(), _settings, out _);
        board[to.AsSquare()].Piece.Should().Be(Piece.BlackKing);
    }

    [Theory]
    [InlineData(6, 3), InlineData(6, 2)]
    [InlineData(5, 2), InlineData(5, 1)]
    [InlineData(9, 4), InlineData(9, 8)]
    public void WhiteManPromotesOnFirstLine(int from, int to) {
        //   |.|.|.|    Side view   |03|07|12|           E
        //  |.|5|.|.|              |02|06|11|16|      N  +  S
        // |.|5|.|.|.|  <- You    |01|05|10|15|19|       W
        //  |.|5|.|.|              |04|09|14|18|
        //   |.|.|.|                |08|13|17|
        var board = Board.FromString(HEX3, "000 0550 05000 0000 000");
        board.PerformNewMove(from.AsSquare(), to.AsSquare(), _settings, out _);
        board[to.AsSquare()].Piece.Should().Be(Piece.WhiteKing);
    }

    [Fact]
    public void BlackManDoesntPromoteOnFirstLine() {
        //   |.|.|.|    Side view   |03|07|12|           E
        //  |.|5|.|.|              |02|06|11|16|      N  +  S
        // |.|.|4|.|.|  <- You    |01|05|10|15|19|       W
        //  |.|.|.|.|              |04|09|14|18|
        //   |.|.|.|                |08|13|17|
        var board = Board.FromString(HEX3, "000 0050 00400 0000 000");
        board.PerformNewMove(10.AsSquare(), 3.AsSquare(), _settings, out _);
        board.ToLongString(" ", "").Should().Be("004 0000 00000 0000 000");
    }

    [Fact]
    public void KingStaysAsHeIs() {
        //   |.|.|.|    Side view   |03|07|12|           E
        //  |.|.|.|.|              |02|06|11|16|      N  +  S
        // |.|.|.|.|.|  <- You    |01|05|10|15|19|       W
        //  |.|.|6|.|              |04|09|14|18|
        //   |.|.|.|                |08|13|17|
        var board = Board.FromString(HEX3, "000 0000 00000 0600 000");
        board.PerformNewMove(14.AsSquare(), 17.AsSquare(), _settings, out _);
        board.ToLongString(" ", "").Should().Be("000 0000 00000 0000 600");
    }

    [Fact]
    public void DontPromoteInsideChainCaptureFromNormalMove() {
        //   |.|.|.|    Side view   |03|07|12|           E
        //  |.|.|.|.|              |02|06|11|16|      N  +  S
        // |.|.|.|5|.|  <- You    |01|05|10|15|19|       W
        //  |.|4|5|.|              |04|09|14|18|
        //   |.|.|.|                |08|13|17|
        var board = Board.FromString(HEX3, "000 0000 04000 0550 000");
        board.PerformNewMove(9.AsSquare(), 18.AsSquare(), _settings, out bool canCaptureMore);
        board.ToLongString(" ", "").Should().Be("000 0000 00000 0D50 040");
        canCaptureMore.Should().BeTrue();
    }

    [Fact]
    public void DontPromoteInsideChainCaptureFromChainCapture() {
        //   |.|.|.|    Side view   |03|07|12|           E
        //  |.|4|5|.|              |02|06|11|16|      N  +  S
        // |.|.|.|5|.|  <- You    |01|05|10|15|19|       W
        //  |.|.|.|.|              |04|09|14|18|
        //   |.|.|.|                |08|13|17|
        var board = Board.FromString(HEX3, "000 0040 00050 0050 000");
        board.PerformChainCaptureMove(6.AsSquare(), 16.AsSquare(), _settings, out bool canCaptureMore);
        board.ToLongString(" ", "").Should().Be("000 0000 000D0 0054 000");
        canCaptureMore.Should().BeTrue();
    }
}
