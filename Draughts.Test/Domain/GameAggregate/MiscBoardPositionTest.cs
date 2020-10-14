using Draughts.Domain.GameAggregate.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Draughts.Test.Domain.GameAggregate {
    [TestClass]
    public class MiscBoardPositionTest {
        [TestMethod]
        public void InitialBoard4x4() {
            var board = BoardPosition.InitialSetup(4);

            board.Width.Should().Be(4);
            board.Height.Should().Be(4);

            for (int y = 0; y < board.Height; y++) {
                for (int x = 0; x < board.Width; x++) {
                    if (y == 0 && (x == 1 || x == 3)) {
                        board[x, y].Should().Be(Piece.BlackMan);
                    }
                    else if (y == 3 && (x == 0 || x == 2)) {
                        board[x, y].Should().Be(Piece.WhiteMan);
                    }
                    else {
                        board[x, y].Should().Be(Piece.Empty, $"because we're at ({x}, {y})");
                    }
                }
            }

            board.NrOfPiecesPerColor(Color.Black).Should().Be(2);
            board.NrOfPiecesPerColor(Color.White).Should().Be(2);
        }

        [TestMethod]
        public void InitialBoard8x8() {
            var board = BoardPosition.InitialSetup(8);
            board.Should().Be(BoardPosition.FromString("4444,4444,4444,0000,0000,5555,5555,5555", ","));
        }

        [TestMethod]
        public void InitialBoard10x10() {
            var board = BoardPosition.InitialSetup(10);
            board.Should().Be(BoardPosition.FromString("44444,44444,44444,44444,00000,00000,55555,55555,55555,55555", ","));
        }

        [TestMethod]
        public void BoardToString() {
            BoardPosition.InitialSetup(4).ToString().Should().Be("44000055");
        }

        [TestMethod]
        public void BoardToLongString() {
            BoardPosition.InitialSetup(4).ToLongString(",").Should().Be(" 4 4,0 0 , 0 0,5 5 ");
        }

        [TestMethod]
        public void StringToBoard() {
            BoardPosition.FromString("44000055").Should().Be(BoardPosition.InitialSetup(4));
        }

        [TestMethod]
        public void LongStringToBoard() {
            BoardPosition.FromString(" 4 4,0 0 , 0 0,5 5 ", ",").Should().Be(BoardPosition.InitialSetup(4));
        }

        [TestMethod]
        [DataRow(0, 0, false), DataRow(1, 0, true), DataRow(2, 0, false), DataRow(3, 0, true)]
        [DataRow(4, 0, false), DataRow(5, 0, true), DataRow(6, 0, false), DataRow(7, 0, true)]
        [DataRow(0, 1, true), DataRow(1, 1, false), DataRow(2, 1, true), DataRow(3, 1, false)]
        [DataRow(6, 1, true), DataRow(7, 1, false)]
        [DataRow(0, 2, false), DataRow(1, 2, true)]
        [DataRow(2, 3, true), DataRow(3, 3, false)]
        [DataRow(6, 7, true), DataRow(7, 7, false)]
        public void IsPlayable(int x, int y, bool expectedResult) {
            BoardPosition.IsPlayable(x, y).Should().Be(expectedResult);
        }
    }
}
