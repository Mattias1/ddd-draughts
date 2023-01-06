using Draughts.Application.Shared.ViewModels;
using Draughts.Domain.GameContext.Models;
using FluentAssertions;
using Xunit;

namespace Draughts.Test.Application.ViewModels;

public sealed class BoardViewModelTest {
    private static readonly IBoardType SQUARE4 = new SquareBoardType(4);
    private static readonly IBoardType HEX3 = new HexagonalBoardType(3);

    [Fact]
    public void GetPiecesForNormalBoard() {
        // |_|4|_|6|
        // |.|_|.|_|
        // |_|.|_|.|
        // |7|_|5|_|
        var boardViewModel = new BoardViewModel(Board.FromString(SQUARE4, "46000075"));
        boardViewModel.At(0, 0, false).Should().BeNull();
        boardViewModel.At(1, 0, false).Should().Be(Piece.BlackMan);
        boardViewModel.At(3, 0, false).Should().Be(Piece.BlackKing);
        boardViewModel.At(0, 1, false).Should().Be(Piece.Empty);
        boardViewModel.At(2, 1, false).Should().Be(Piece.Empty);
        boardViewModel.At(1, 2, false).Should().Be(Piece.Empty);
        boardViewModel.At(3, 2, false).Should().Be(Piece.Empty);
        boardViewModel.At(0, 3, false).Should().Be(Piece.WhiteKing);
        boardViewModel.At(2, 3, false).Should().Be(Piece.WhiteMan);

        // |_|.|_|.|
        // |4|_|6|_|
        // |_|7|_|5|
        // |.|_|.|_|
        boardViewModel = new BoardViewModel(Board.FromString(SQUARE4, "00467500"));
        boardViewModel.At(0, 0, false).Should().BeNull();
        boardViewModel.At(1, 0, false).Should().Be(Piece.Empty);
        boardViewModel.At(3, 0, false).Should().Be(Piece.Empty);
        boardViewModel.At(0, 1, false).Should().Be(Piece.BlackMan);
        boardViewModel.At(2, 1, false).Should().Be(Piece.BlackKing);
        boardViewModel.At(1, 2, false).Should().Be(Piece.WhiteKing);
        boardViewModel.At(3, 2, false).Should().Be(Piece.WhiteMan);
        boardViewModel.At(0, 3, false).Should().Be(Piece.Empty);
        boardViewModel.At(2, 3, false).Should().Be(Piece.Empty);
    }

    [Fact]
    public void GetPiecesForRotatedBoard() {
        // |_|4|_|6|
        // |.|_|.|_|
        // |_|.|_|.|
        // |7|_|5|_|
        var board = new BoardViewModel(Board.FromString(SQUARE4, "46000075"));
        board.At(0, 0, true).Should().BeNull();
        board.At(1, 0, true).Should().Be(Piece.WhiteMan);
        board.At(3, 0, true).Should().Be(Piece.WhiteKing);
        board.At(0, 1, true).Should().Be(Piece.Empty);
        board.At(2, 1, true).Should().Be(Piece.Empty);
        board.At(1, 2, true).Should().Be(Piece.Empty);
        board.At(3, 2, true).Should().Be(Piece.Empty);
        board.At(0, 3, true).Should().Be(Piece.BlackKing);
        board.At(2, 3, true).Should().Be(Piece.BlackMan);

        // |_|.|_|.|
        // |4|_|6|_|
        // |_|7|_|5|
        // |.|_|.|_|
        board = new BoardViewModel(Board.FromString(SQUARE4, "00467500"));
        board.At(0, 0, true).Should().BeNull();
        board.At(1, 0, true).Should().Be(Piece.Empty);
        board.At(3, 0, true).Should().Be(Piece.Empty);
        board.At(0, 1, true).Should().Be(Piece.WhiteMan);
        board.At(2, 1, true).Should().Be(Piece.WhiteKing);
        board.At(1, 2, true).Should().Be(Piece.BlackKing);
        board.At(3, 2, true).Should().Be(Piece.BlackMan);
        board.At(0, 3, true).Should().Be(Piece.Empty);
        board.At(2, 3, true).Should().Be(Piece.Empty);
    }

    [Fact]
    public void FirstSquareIdOfRowForNormalBoard() {
        // |_|1|_|2|
        // |3|_|4|_|
        // |_|5|_|6|
        // |7|_|8|_|
        var board = new BoardViewModel(Board.InitialSetup(SQUARE4));
        board.FirstSquareIdOfRow(0, false).Value.Should().Be(1);
        board.FirstSquareIdOfRow(1, false).Value.Should().Be(3);
        board.FirstSquareIdOfRow(2, false).Value.Should().Be(5);
        board.FirstSquareIdOfRow(3, false).Value.Should().Be(7);
    }

    [Fact]
    public void FirstSquareIdOfRowForRotatedBoard() {
        // |_|8|_|7|
        // |6|_|5|_|
        // |_|4|_|3|
        // |2|_|1|_|
        var board = new BoardViewModel(Board.InitialSetup(SQUARE4));
        board.FirstSquareIdOfRow(0, true).Value.Should().Be(8);
        board.FirstSquareIdOfRow(1, true).Value.Should().Be(6);
        board.FirstSquareIdOfRow(2, true).Value.Should().Be(4);
        board.FirstSquareIdOfRow(3, true).Value.Should().Be(2);
    }

    [Fact]
    public void LastSquareIdOfColForNormalBoard() {
        // |_|1|_|2|
        // |3|_|4|_|
        // |_|5|_|6|
        // |7|_|8|_|
        var board = new BoardViewModel(Board.InitialSetup(SQUARE4));
        board.LastSquareIdOfCol(0, false).Value.Should().Be(7);
        board.LastSquareIdOfCol(1, false).Value.Should().Be(5);
        board.LastSquareIdOfCol(2, false).Value.Should().Be(8);
        board.LastSquareIdOfCol(3, false).Value.Should().Be(6);
    }

    [Fact]
    public void LastSquareIdOfColForRotatedBoard() {
        // |_|8|_|7|
        // |6|_|5|_|
        // |_|4|_|3|
        // |2|_|1|_|
        var board = new BoardViewModel(Board.InitialSetup(SQUARE4));
        board.LastSquareIdOfCol(0, true).Value.Should().Be(2);
        board.LastSquareIdOfCol(1, true).Value.Should().Be(4);
        board.LastSquareIdOfCol(2, true).Value.Should().Be(1);
        board.LastSquareIdOfCol(3, true).Value.Should().Be(3);
    }

    [Fact]
    public void FirstSquareIdOfRowForNormalHexagonalBoard() {
        //   |03|07|12|      Side view    E
        //  |02|06|11|16|              N  +  S
        // |01|05|10|15|19|  <- You       W
        //  |04|09|14|18|
        //   |08|13|17|
        var board = new BoardViewModel(Board.InitialSetup(HEX3));
        board.FirstSquareIdOfRow(0, false).Value.Should().Be(1);
        board.FirstSquareIdOfRow(1, false).Value.Should().Be(4);
        board.FirstSquareIdOfRow(2, false).Value.Should().Be(8);
        board.FirstSquareIdOfRow(3, false).Value.Should().Be(13);
        board.FirstSquareIdOfRow(4, false).Value.Should().Be(17);
    }

    [Fact]
    public void FirstSquareIdOfRowForRotatedHexagonalBoard() {
        //   |17|13|08|      Side view    E
        //  |18|14|09|04|              N  +  S
        // |19|15|10|05|01|  <- You       W
        //  |16|11|06|02|
        //   |12|07|03|
        var board = new BoardViewModel(Board.InitialSetup(HEX3));
        board.FirstSquareIdOfRow(0, true).Value.Should().Be(19);
        board.FirstSquareIdOfRow(1, true).Value.Should().Be(16);
        board.FirstSquareIdOfRow(2, true).Value.Should().Be(12);
        board.FirstSquareIdOfRow(3, true).Value.Should().Be(7);
        board.FirstSquareIdOfRow(4, true).Value.Should().Be(3);
    }

    [Fact]
    public void LastSquareIdOfColForNormalHexagonalBoard() {
        //   |03|07|12|      Side view    E
        //  |02|06|11|16|              N  +  S
        // |01|05|10|15|19|  <- You       W
        //  |04|09|14|18|
        //   |08|13|17|
        var board = new BoardViewModel(Board.InitialSetup(HEX3));
        board.LastSquareIdOfCol(0, false).Value.Should().Be(17);
        board.LastSquareIdOfCol(1, false).Value.Should().Be(18);
        board.LastSquareIdOfCol(2, false).Value.Should().Be(19);
        board.LastSquareIdOfCol(3, false).Value.Should().Be(16);
        board.LastSquareIdOfCol(4, false).Value.Should().Be(12);
    }

    [Fact]
    public void LastSquareIdOfColForRotatedHexagonalBoard() {
        //   |17|13|08|      Side view    E
        //  |18|14|09|04|              N  +  S
        // |19|15|10|05|01|  <- You       W
        //  |16|11|06|02|
        //   |12|07|03|
        var board = new BoardViewModel(Board.InitialSetup(HEX3));
        board.LastSquareIdOfCol(0, true).Value.Should().Be(3);
        board.LastSquareIdOfCol(1, true).Value.Should().Be(2);
        board.LastSquareIdOfCol(2, true).Value.Should().Be(1);
        board.LastSquareIdOfCol(3, true).Value.Should().Be(4);
        board.LastSquareIdOfCol(4, true).Value.Should().Be(8);
    }
}
