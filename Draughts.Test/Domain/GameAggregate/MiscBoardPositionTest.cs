using Draughts.Domain.GameAggregate.Models;
using FluentAssertions;
using Xunit;

namespace Draughts.Test.Domain.GameAggregate {
    public class MiscBoardPositionTest {
        [Fact]
        public void InitialBoard4x4() {
            var board = BoardPosition.InitialSetup(4);

            board.Size.Should().Be(4);

            for (int y = 0; y < board.Size; y++) {
                for (int x = 0; x < board.Size; x++) {
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

        [Fact]
        public void InitialBoard8x8() {
            var board = BoardPosition.InitialSetup(8);
            board.Should().Be(BoardPosition.FromString("4444,4444,4444,0000,0000,5555,5555,5555", ","));
        }

        [Fact]
        public void InitialBoard10x10() {
            var board = BoardPosition.InitialSetup(10);
            board.Should().Be(BoardPosition.FromString("44444,44444,44444,44444,00000,00000,55555,55555,55555,55555", ","));
        }

        [Fact]
        public void BoardToString() {
            BoardPosition.InitialSetup(4).ToString().Should().Be("44000055");
        }

        [Fact]
        public void BoardToLongString() {
            BoardPosition.InitialSetup(4).ToLongString(",").Should().Be(" 4 4,0 0 , 0 0,5 5 ");
        }

        [Fact]
        public void StringToBoard() {
            BoardPosition.FromString("44000055").Should().Be(BoardPosition.InitialSetup(4));
        }

        [Fact]
        public void LongStringToBoard() {
            BoardPosition.FromString(" 4 4,0 0 , 0 0,5 5 ", ",").Should().Be(BoardPosition.InitialSetup(4));
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
            BoardPosition.IsPlayable(x, y).Should().Be(expectedResult);
        }
    }
}
