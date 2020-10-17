using Draughts.Domain.GameAggregate.Models;
using FluentAssertions;
using Xunit;

namespace Draughts.Test.Domain.GameAggregate {
    public class InternationalBoardPositionTest {
        [Fact]
        public void CanMoveForwardsToAdjacentSquare() {
            var board = BoardPosition.FromString("40 00 00 00");
            board.IsMove(Pos(1, 0), Pos(2, 1)).Should().BeTrue();
        }

        [Fact]
        public void CantMoveToOccupiedSquare() {
            var board = BoardPosition.FromString("40 04 00 00");
            board.IsMove(Pos(1, 0), Pos(2, 1)).Should().BeFalse();
        }

        [Fact]
        public void CantMoveFromEmptySquare() {
            var board = BoardPosition.FromString("00 00 00 00");
            board.IsMove(Pos(1, 0), Pos(2, 1)).Should().BeFalse();
        }

        [Fact(Skip = "Not implemented yet.")]
        public void ManCantMoveBackwards() {
            var board = BoardPosition.FromString("00 04 00 00");
            board.IsMove(Pos(2, 1), Pos(1, 0)).Should().BeFalse();
        }

        [Fact]
        public void ManCantFly() {
            var board = BoardPosition.FromString("40 00 00 00");
            board.IsMove(Pos(1, 0), Pos(3, 2)).Should().BeFalse();
        }

        [Fact(Skip = "Not implemented")]
        public void KingCanFly() {
            var board = BoardPosition.FromString("06 00 00 00");
            board.IsMove(Pos(3, 0), Pos(0, 3)).Should().BeTrue();
        }

        [Fact(Skip = "This test passes, but for the wrong reasons. It'd throw right now if flying kings are implemented.")]
        public void CantMoveKingOffTheDiagonal() {
            var board = BoardPosition.FromString("60 00 00 00");
            board.IsMove(Pos(1, 0), Pos(2, 3)).Should().BeTrue();
        }

        [Fact]
        public void PerformMoveManForwards() {
            var board = BoardPosition.FromString("40 00 00 00");
            board.Move(Pos(1, 0), Pos(2, 1));
            board.ToLongString(" ", "").Should().Be("00 04 00 00");
        }

        [Fact]
        public void ManCanCaptureForwards() {
            var board = BoardPosition.FromString("04 05 00 00");
            board.IsCapture(Pos(3, 0), Pos(1, 2)).Should().BeTrue();
        }

        [Fact]
        public void ManCanCaptureBackwards() {
            var board = BoardPosition.FromString("00 05 40 00");
            board.IsCapture(Pos(1, 2), Pos(3, 0)).Should().BeTrue();
        }

        [Fact]
        public void CantCaptureToOccupiedSquare() {
            var board = BoardPosition.FromString("04 05 40 00");
            board.IsCapture(Pos(3, 0), Pos(1, 2)).Should().BeFalse();
        }

        [Fact]
        public void CantCaptureFromEmptySquare() {
            var board = BoardPosition.FromString("00 05 00 00");
            board.IsCapture(Pos(3, 0), Pos(1, 2)).Should().BeFalse();
        }

        [Fact]
        public void ManCantCaptureFlying() {
            var board = BoardPosition.FromString("04 00 50 00");
            board.IsMove(Pos(3, 0), Pos(0, 3)).Should().BeFalse();
        }

        [Fact]
        public void ManCantFlyAfterCapture() {
            var board = BoardPosition.FromString("04 05 00 00");
            board.IsMove(Pos(3, 0), Pos(0, 3)).Should().BeFalse();
        }

        private SquareNumber Pos(int x, int y) => SquareNumber.FromPosition(x, y, 4);
    }
}