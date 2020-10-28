using Draughts.Domain.GameAggregate.Models;
using FluentAssertions;
using Xunit;

namespace Draughts.Test.Domain.GameAggregate {
    public class InternationalBoardPositionTest {
        [Fact]
        public void CanMoveForwardsToAdjacentSquare() {
            Board("40 00 00 00").IsMove(Pos(1, 0), Pos(2, 1)).Should().BeTrue();
        }

        [Fact]
        public void CantMoveToOccupiedSquare() {
            Board("40 04 00 00").IsMove(Pos(1, 0), Pos(2, 1)).Should().BeFalse();
        }

        [Fact]
        public void CantMoveFromEmptySquare() {
            Board("00 00 00 00").IsMove(Pos(1, 0), Pos(2, 1)).Should().BeFalse();
        }

        [Fact]
        public void ManCantMoveBackwards() {
            Board("00 04 00 00").IsMove(Pos(2, 1), Pos(1, 0)).Should().BeFalse();
        }

        [Fact]
        public void ManCantFly() {
            Board("40 00 00 00").IsMove(Pos(1, 0), Pos(3, 2)).Should().BeFalse();
        }

        [Fact]
        public void PerformMoveManForwards() {
            var board = Board("40 00 00 00");
            board.Move(Pos(1, 0), Pos(2, 1));
            board.ToLongString(" ", "").Should().Be("00 04 00 00");
        }

        [Fact]
        public void ManCanCaptureForwards() {
            Board("04 05 00 00").IsCapture(Pos(3, 0), Pos(1, 2)).Should().BeTrue();
        }

        [Fact]
        public void ManCanCaptureBackwards() {
            Board("00 05 40 00").IsCapture(Pos(1, 2), Pos(3, 0)).Should().BeTrue();
        }

        [Fact]
        public void CantCaptureToOccupiedSquare() {
            Board("04 05 40 00").IsCapture(Pos(3, 0), Pos(1, 2)).Should().BeFalse();
        }

        [Fact]
        public void CantCaptureFromEmptySquare() {
            Board("00 05 00 00").IsCapture(Pos(3, 0), Pos(1, 2)).Should().BeFalse();
        }

        [Fact]
        public void ManCantCaptureFlying() {
            Board("04 00 50 00").IsMove(Pos(3, 0), Pos(0, 3)).Should().BeFalse();
        }

        [Fact]
        public void ManCantFlyAfterCapture() {
            Board("04 05 00 00").IsMove(Pos(3, 0), Pos(0, 3)).Should().BeFalse();
        }

        [Fact]
        public void PerformManCaptureForwards() {
            var board = Board("40 05 00 00");
            board.Capture(Pos(1, 0), Pos(3, 2));
            board.ToLongString(" ", "").Should().Be("00 00 04 00");
        }

        [Fact]
        public void BlackManCanPromoteOnLastLine() {
            Board("00 00 00 40").CanPromote(Pos(0, 3)).Should().BeTrue();
        }

        [Fact]
        public void WhiteManCanPromoteOnFirstLine() {
            Board("05 00 00 00").CanPromote(Pos(3, 0)).Should().BeTrue();
        }

        [Fact]
        public void BlackManCantPromoteOnFirstLine() {
            Board("40 00 00 00").CanPromote(Pos(3, 0)).Should().BeFalse();
        }

        [Fact]
        public void KingCantPromote() {
            Board("00 00 00 60").CanPromote(Pos(0, 3)).Should().BeFalse();
        }

        [Fact]
        public void KingCanMoveToOccupiedSquare() {
            Board("60 04 00 00").IsMove(Pos(1, 0), Pos(2, 1)).Should().BeFalse();
        }

        [Fact]
        public void KingCanFly() {
            Board("06 00 00 00").IsMove(Pos(3, 0), Pos(0, 3)).Should().BeTrue();
        }

        [Fact]
        public void CantMoveKingOffTheDiagonal() {
            Board("60 00 00 00").IsMove(Pos(1, 0), Pos(2, 3)).Should().BeFalse();
        }

        [Fact]
        public void KingFliesToCapture() {
            Board("00 04 00 70").IsCapture(Pos(0, 3), Pos(3, 0)).Should().BeTrue();
        }

        [Fact]
        public void KingFliesPastCapture() {
            Board("00 00 40 70").IsCapture(Pos(0, 3), Pos(3, 0)).Should().BeTrue();
        }

        [Fact]
        public void KingCantCaptureMultiple() {
            Board("00 04 40 70").IsCapture(Pos(0, 3), Pos(3, 0)).Should().BeFalse();
        }

        [Fact]
        public void KingCantCaptureOccupiedSquare() {
            Board("05 04 00 70").IsCapture(Pos(0, 3), Pos(3, 0)).Should().BeFalse();
        }

        [Fact]
        public void PerformKingCaptureForwards() {
            var board = Board("00 04 00 70");
            board.Capture(Pos(0, 3), Pos(3, 0));
            board.ToLongString(" ", "").Should().Be("07 00 00 00");
        }

        private BoardPosition Board(string board) => BoardPosition.FromString(board);

        private SquareNumber Pos(int x, int y) => SquareNumber.FromPosition(x, y, 4);
    }
}
