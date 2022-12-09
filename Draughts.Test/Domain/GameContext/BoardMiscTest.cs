using Draughts.Domain.GameContext.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using Xunit;

namespace Draughts.Test.Domain.GameContext;

public sealed class BoardMiscTest {
    [Fact]
    public void InitialBoard4x4() {
        // |_|4|_|4|
        // |.|_|.|_|
        // |_|.|_|.|
        // |5|_|5|_|
        var board = Board.InitialSetup(4);

        board.Size.Should().Be(4);

        for (int y = 0; y < board.Size; y++) {
            for (int x = 0; x < board.Size; x++) {
                if (y == 0 && (x == 1 || x == 3)) {
                    board[x, y]!.Piece.Should().Be(Piece.BlackMan);
                }
                else if (y == 3 && (x == 0 || x == 2)) {
                    board[x, y]!.Piece.Should().Be(Piece.WhiteMan);
                }
                else if (Board.IsPlayable(x, y)) {
                    board[x, y]!.Piece.Should().Be(Piece.Empty, $"because we're at ({x}, {y})");
                }
                else {
                    board[x, y].Should().BeNull($"because we're at ({x}, {y})");
                }
            }
        }

        board.NrOfPiecesPerColor(Color.Black).Should().Be(2);
        board.NrOfPiecesPerColor(Color.White).Should().Be(2);
    }

    [Fact]
    public void InitialBoard8x8() {
        var board = Board.InitialSetup(8);
        board.Should().Be(Board.FromString("4444,4444,4444,0000,0000,5555,5555,5555", ","));
    }

    [Fact]
    public void InitialBoard10x10() {
        var board = Board.InitialSetup(10);
        board.Should().Be(Board.FromString("44444,44444,44444,44444,00000,00000,55555,55555,55555,55555", ","));
    }

    [Fact]
    public void BoardToString() {
        Board.InitialSetup(4).ToString().Should().Be("44000055");
    }

    [Fact]
    public void BoardToLongString() {
        Board.InitialSetup(4).ToLongString(",").Should().Be(" 4 4,0 0 , 0 0,5 5 ");
    }

    [Fact]
    public void StringToBoard() {
        Board.FromString("44000055").Should().Be(Board.InitialSetup(4));
    }

    [Fact]
    public void LongStringToBoard() {
        Board.FromString(" 4 4,0 0 , 0 0,5 5 ", ",").Should().Be(Board.InitialSetup(4));
    }

    [Theory]
    [InlineData(0, 0, false), InlineData(1, 0, true), InlineData(2, 0, false), InlineData(3, 0, true)]
    [InlineData(4, 0, false), InlineData(5, 0, true), InlineData(6, 0, false), InlineData(7, 0, true)]
    [InlineData(0, 1, true), InlineData(1, 1, false), InlineData(2, 1, true), InlineData(3, 1, false)]
    [InlineData(6, 1, true), InlineData(7, 1, false)]
    [InlineData(0, 2, false), InlineData(1, 2, true)]
    [InlineData(2, 3, true), InlineData(3, 3, false)]
    [InlineData(6, 7, true), InlineData(7, 7, false)]
    public void IsPlayable(int x, int y, bool expectedResult) {
        Board.IsPlayable(x, y).Should().Be(expectedResult);
    }

    [Fact]
    public void CopiedBoardMatchesOriginal() {
        var board = Board.FromString("440 404 000 050 055 007");
        var copy = board.Copy();
        copy.Should().Be(board);
    }

    [Fact]
    public void CopiedBoardDoesntModifyOriginal() {
        var board = Board.FromString("440 404 000 050 055 007");
        var copy = board.Copy();
        copy.PerformNewMove(4.AsSquare(), 7.AsSquare(), GameSettings.International, out bool canCaptureMore);
        board.ToLongString(" ", "").Should().Be("440 404 000 050 055 007");
        copy.ToLongString(" ", "").Should().Be("440 004 400 050 055 007");
    }
}