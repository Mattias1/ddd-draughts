using Draughts.Common;
using Draughts.Domain.GameContext.Models;
using FluentAssertions;
using System;
using Xunit;

namespace Draughts.Test.Domain.GameContext {
    public class BoardPositionMoveTest {
        private readonly GameSettings _settings = GameSettings.International;

        [Fact]
        public void InvalidMoveTrows() {
            // |_|4|_|.|
            // |.|_|.|_|
            // |_|.|_|.|
            // |.|_|5|_|
            var board = Board.FromString("40 00 00 05");
            Action doMove = () => Move(board, 1, 5, out bool _);
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
            Move(board, 1, 3, out bool canCaptureMore);
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
            Move(board, 1, 6, out bool canCaptureMore);
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
            Action doMove = () => board.PerformChainCaptureMove(new SquareId(1), new SquareId(2), _settings, out bool _);
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
            Action doMove = () => board.PerformChainCaptureMove(new SquareId(1), new SquareId(3), _settings, out bool _);
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
            board.PerformChainCaptureMove(new SquareId(1), new SquareId(6), _settings, out bool canCaptureMore);
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
            Move(board, 5, 7, out bool _);
            board.ToLongString(" ", "").Should().Be("00 00 00 60");
        }

        [Fact]
        public void WhiteManPromotesOnFirstLine() {
            // |_|.|_|.|
            // |.|_|5|_|
            // |_|.|_|.|
            // |.|_|.|_|
            var board = Board.FromString("00 05 00 00");
            Move(board, 4, 2, out bool _);
            board.ToLongString(" ", "").Should().Be("07 00 00 00");
        }

        [Fact]
        public void BlackManDoesntPromoteOnFirstLine() {
            // |_|.|_|.|
            // |.|_|5|_|
            // |_|.|_|4|
            // |.|_|.|_|
            var board = Board.FromString("00 05 04 00");
            Move(board, 6, 1, out bool _);
            board.ToLongString(" ", "").Should().Be("40 00 00 00");
        }

        [Fact]
        public void KingStaysAsHeIs() {
            // |_|.|_|.|
            // |.|_|.|_|
            // |_|6|_|.|
            // |.|_|.|_|
            var board = Board.FromString("00 00 60 00");
            Move(board, 5, 7, out bool _);
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
            Move(board, 7, 2, out bool canCaptureMore);
            board.ToLongString(" ", "").Should().Be("050 004 000 000 000 000");
            canCaptureMore.Should().BeTrue();
        }

        private void Move(Board board, int from, int to, out bool canCaptureMore) {
            board.PerformNewMove(new SquareId(from), new SquareId(to), _settings, out canCaptureMore);
        }
    }
}
