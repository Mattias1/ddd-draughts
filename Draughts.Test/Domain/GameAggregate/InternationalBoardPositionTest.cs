using Draughts.Domain.GameAggregate.Models;
using FluentAssertions;
using Xunit;

namespace Draughts.Test.Domain.GameAggregate {
    public class InternationalBoardPositionTest {
        [Fact]
        public void CanMoveForwardsToAdjacentSquare() {
            /*|_|4|_|.|
              |.|_|.|_|
              |_|.|_|.|
              |.|_|.|_|*/
            Board("40 00 00 00").IsMove(Pos(1, 0), Pos(2, 1)).Should().BeTrue();
        }

        [Fact]
        public void CantMoveToOccupiedSquare() {
            /*|_|4|_|.|
              |.|_|4|_|
              |_|.|_|.|
              |.|_|.|_|*/
            Board("40 04 00 00").IsMove(Pos(1, 0), Pos(2, 1)).Should().BeFalse();
        }

        [Fact]
        public void CantMoveFromEmptySquare() {
            /*|_|.|_|.|
              |.|_|.|_|
              |_|.|_|.|
              |.|_|.|_|*/
            Board("00 00 00 00").IsMove(Pos(1, 0), Pos(2, 1)).Should().BeFalse();
        }

        [Fact]
        public void ManCantMoveBackwards() {
            /*|_|.|_|.|
              |.|_|4|_|
              |_|.|_|.|
              |.|_|.|_|*/
            Board("00 04 00 00").IsMove(Pos(2, 1), Pos(1, 0)).Should().BeFalse();
        }

        [Fact]
        public void ManCantFly() {
            /*|_|4|_|.|
              |.|_|.|_|
              |_|.|_|.|
              |.|_|.|_|*/
            Board("40 00 00 00").IsMove(Pos(1, 0), Pos(3, 2)).Should().BeFalse();
        }

        [Fact]
        public void PerformMoveManForwards() {
            /*|_|4|_|.|
              |.|_|.|_|
              |_|.|_|.|
              |.|_|.|_|*/
            var board = Board("40 00 00 00");
            board.Move(Pos(1, 0), Pos(2, 1));
            board.ToLongString(" ", "").Should().Be("00 04 00 00");
        }

        [Fact]
        public void ManCanCaptureForwards() {
            /*|_|.|_|4|
              |.|_|5|_|
              |_|.|_|.|
              |.|_|.|_|*/
            Board("04 05 00 00").IsCapture(Pos(3, 0), Pos(1, 2)).Should().BeTrue();
        }

        [Fact]
        public void ManCanCaptureBackwards() {
            /*|_|.|_|.|
              |.|_|5|_|
              |_|4|_|.|
              |.|_|.|_|*/
            Board("00 05 40 00").IsCapture(Pos(1, 2), Pos(3, 0)).Should().BeTrue();
        }

        [Fact]
        public void CantCaptureToOccupiedSquare() {
            /*|_|.|_|4|
              |.|_|5|_|
              |_|4|_|.|
              |.|_|.|_|*/
            Board("04 05 40 00").IsCapture(Pos(3, 0), Pos(1, 2)).Should().BeFalse();
        }

        [Fact]
        public void CantCaptureFromEmptySquare() {
            /*|_|.|_|.|
              |.|_|5|_|
              |_|.|_|.|
              |.|_|.|_|*/
            Board("00 05 00 00").IsCapture(Pos(3, 0), Pos(1, 2)).Should().BeFalse();
        }

        [Fact]
        public void ManCantCaptureFlying() {
            /*|_|.|_|4|
              |.|_|.|_|
              |_|5|_|.|
              |.|_|.|_|*/
            Board("04 00 50 00").IsMove(Pos(3, 0), Pos(0, 3)).Should().BeFalse();
        }

        [Fact]
        public void ManCantFlyAfterCapture() {
            /*|_|.|_|4|
              |.|_|5|_|
              |_|.|_|.|
              |.|_|.|_|*/
            Board("04 05 00 00").IsMove(Pos(3, 0), Pos(0, 3)).Should().BeFalse();
        }

        [Fact]
        public void PerformManCaptureForwards() {
            /*|_|4|_|.|
              |.|_|5|_|
              |_|.|_|.|
              |.|_|.|_|*/
            var board = Board("40 05 00 00");
            board.Capture(Pos(1, 0), Pos(3, 2));
            board.ToLongString(" ", "").Should().Be("00 00 04 00");
        }

        [Fact]
        public void BlackManCanPromoteOnLastLine() {
            /*|_|.|_|.|
              |.|_|.|_|
              |_|.|_|.|
              |4|_|.|_|*/
            Board("00 00 00 40").CanPromote(Pos(0, 3)).Should().BeTrue();
        }

        [Fact]
        public void WhiteManCanPromoteOnFirstLine() {
            /*|_|.|_|5|
              |.|_|.|_|
              |_|.|_|.|
              |5|_|.|_|*/
            Board("05 00 00 00").CanPromote(Pos(3, 0)).Should().BeTrue();
        }

        [Fact]
        public void BlackManCantPromoteOnFirstLine() {
            /*|_|4|_|.|
              |.|_|.|_|
              |_|.|_|.|
              |.|_|.|_|*/
            Board("40 00 00 00").CanPromote(Pos(3, 0)).Should().BeFalse();
        }

        [Fact]
        public void KingCantPromote() {
            /*|_|.|_|.|
              |.|_|.|_|
              |_|.|_|.|
              |6|_|.|_|*/
            Board("00 00 00 60").CanPromote(Pos(0, 3)).Should().BeFalse();
        }

        [Fact]
        public void KingCanMoveToOccupiedSquare() {
            /*|_|6|_|.|
              |.|_|4|_|
              |_|.|_|.|
              |6|_|.|_|*/
            Board("60 04 00 00").IsMove(Pos(1, 0), Pos(2, 1)).Should().BeFalse();
        }

        [Fact]
        public void KingCanFly() {
            /*|_|.|_|6|
              |.|_|.|_|
              |_|.|_|.|
              |6|_|.|_|*/
            Board("06 00 00 00").IsMove(Pos(3, 0), Pos(0, 3)).Should().BeTrue();
        }

        [Fact]
        public void CantMoveKingOffTheDiagonal() {
            /*|_|6|_|.|
              |.|_|.|_|
              |_|.|_|.|
              |6|_|.|_|*/
            Board("60 00 00 00").IsMove(Pos(1, 0), Pos(2, 3)).Should().BeFalse();
        }

        [Fact]
        public void KingFliesToCapture() {
            /*|_|.|_|.|
              |.|_|4|_|
              |_|.|_|.|
              |7|_|.|_|*/
            Board("00 04 00 70").IsCapture(Pos(0, 3), Pos(3, 0)).Should().BeTrue();
        }

        [Fact]
        public void KingFliesPastCapture() {
            /*|_|.|_|.|
              |.|_|.|_|
              |_|4|_|.|
              |7|_|.|_|*/
            Board("00 00 40 70").IsCapture(Pos(0, 3), Pos(3, 0)).Should().BeTrue();
        }

        [Fact]
        public void KingCantCaptureMultiple() {
            /*|_|.|_|.|
              |.|_|4|_|
              |_|4|_|.|
              |7|_|.|_|*/
            Board("00 04 40 70").IsCapture(Pos(0, 3), Pos(3, 0)).Should().BeFalse();
        }

        [Fact]
        public void KingCantCaptureOccupiedSquare() {
            /*|_|.|_|5|
              |.|_|4|_|
              |_|.|_|.|
              |7|_|.|_|*/
            Board("05 04 00 70").IsCapture(Pos(0, 3), Pos(3, 0)).Should().BeFalse();
        }

        [Fact]
        public void PerformKingCaptureForwards() {
            /*|_|.|_|.|
              |.|_|4|_|
              |_|.|_|.|
              |7|_|.|_|*/
            var board = Board("00 04 00 70");
            board.Capture(Pos(0, 3), Pos(3, 0));
            board.ToLongString(" ", "").Should().Be("07 00 00 00");
        }

        private BoardPosition Board(string board) => BoardPosition.FromString(board);

        private Square Pos(int x, int y) => Square.FromPosition(x, y, 4);
    }
}
