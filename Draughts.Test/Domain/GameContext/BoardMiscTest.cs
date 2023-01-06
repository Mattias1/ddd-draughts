using Draughts.Domain.GameContext.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using Xunit;

namespace Draughts.Test.Domain.GameContext;

public sealed class BoardMiscTest {
    private static readonly IBoardType SQUARE4 = new SquareBoardType(4);
    private static readonly IBoardType SQUARE6 = new SquareBoardType(6);
    private static readonly IBoardType SQUARE8 = new SquareBoardType(8);
    private static readonly IBoardType SQUARE10 = new SquareBoardType(10);
    private static readonly IBoardType HEX3 = new HexagonalBoardType(3);
    private static readonly IBoardType HEX5 = new HexagonalBoardType(5);

    [Fact]
    public void InitialBoard4x4() {
        // |_|4|_|4|        |_|1|_|2|
        // |.|_|.|_|        |3|_|4|_|
        // |_|.|_|.|        |_|5|_|6|
        // |5|_|5|_|        |7|_|8|_|
        var board = Board.InitialSetup(SQUARE4);

        board.DisplaySize.Should().Be(4);

        for (int y = 0; y < board.LongSize; y++) {
            for (int x = 0; x < board.LongSize; x++) {
                if (y == 0 && (x == 1 || x == 3)) {
                    board[x, y]!.Piece.Should().Be(Piece.BlackMan);
                } else if (y == 3 && (x == 0 || x == 2)) {
                    board[x, y]!.Piece.Should().Be(Piece.WhiteMan);
                } else if (board.Type.IsPlayable(x, y)) {
                    board[x, y]!.Piece.Should().Be(Piece.Empty, $"because we're at ({x}, {y})");
                } else {
                    board[x, y].Should().BeNull($"because we're at ({x}, {y})");
                }
            }
        }

        board.NrOfPiecesPerColor(Color.Black).Should().Be(2);
        board.NrOfPiecesPerColor(Color.White).Should().Be(2);
    }

    [Fact]
    public void InitialBoard8x8() {
        var board = Board.InitialSetup(SQUARE8);
        board.Should().Be(Board.FromString(SQUARE8, "4444,4444,4444,0000,0000,5555,5555,5555", ","));
    }

    [Fact]
    public void InitialBoard10x10() {
        var board = Board.InitialSetup(SQUARE10);
        board.Should().Be(Board.FromString(SQUARE10, "44444,44444,44444,44444,00000,00000,55555,55555,55555,55555", ","));
    }

    [Fact]
    public void InitialHexdame3Board() {
        //   |.|.|.|    Side view   |03|07|12|           E
        //  |4|.|.|5|              |02|06|11|16|      N  +  S
        // |4|4|.|5|5|  <- You    |01|05|10|15|19|       W
        //  |4|.|.|5|              |04|09|14|18|
        //   |.|.|.|                |08|13|17|
        var board = Board.InitialSetup(HEX3);
        board.Should().Be(Board.FromString(HEX3, "440,4400,00000,0055,055", ","));
    }

    [Fact]
    public void InitialHexdame5Board() {
        var board = Board.InitialSetup(HEX5);
        board.Should().Be(Board.FromString(HEX5,
            "44440,444400,4444000,44440000,000000000,00005555,0005555,005555,05555", ","));
    }

    [Fact]
    public void BoardToString() {
        Board.InitialSetup(SQUARE4).ToString().Should().Be("44000055");
    }

    [Fact]
    public void BoardToLongString() {
        Board.InitialSetup(SQUARE4).ToLongString(",").Should().Be(" 4 4,0 0 , 0 0,5 5 ");
    }

    [Fact]
    public void HexBoardToLongString() {
        Board.InitialSetup(HEX3).ToLongString(",").Should().Be("  440, 4400,00000,0055 ,055  ");
    }

    [Fact]
    public void StringToBoard() {
        Board.FromString(SQUARE4, "44000055").Should().Be(Board.InitialSetup(SQUARE4));
    }

    [Fact]
    public void LongStringToBoard() {
        Board.FromString(SQUARE4, " 4 4,0 0 , 0 0,5 5 ", ",").Should().Be(Board.InitialSetup(SQUARE4));
    }

    [Fact]
    public void LongStringToHexBoard() {
        Board.FromString(HEX3, "  440, 4400,00000,0055 ,055  ", ",").Should().Be(Board.InitialSetup(HEX3));
    }

    [Fact]
    public void CopiedBoardMatchesOriginal() {
        var board = Board.FromString(SQUARE6, "440 404 000 050 055 007");
        var copy = board.Copy();
        copy.Should().Be(board);
    }

    [Fact]
    public void CopiedBoardDoesntModifyOriginal() {
        var board = Board.FromString(SQUARE6, "440 404 000 050 055 007");
        var copy = board.Copy();
        copy.PerformNewMove(4.AsSquare(), 7.AsSquare(), GameSettings.International, out bool _);
        board.ToLongString(" ", "").Should().Be("440 404 000 050 055 007");
        copy.ToLongString(" ", "").Should().Be("440 004 400 050 055 007");
    }
}
