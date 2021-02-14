using Draughts.Common;
using Draughts.Domain.GameAggregate.Models;
using FluentAssertions;
using System;
using Xunit;

namespace Draughts.Test.Domain.GameAggregate {
    public class BoardPositionMoveTest {
        private GameSettings _settings;

        public BoardPositionMoveTest() {
            _settings = GameSettings.International;
        }

        [Fact]
        public void InvalidMoveTrows() {
            // |_|4|_|.|
            // |.|_|.|_|
            // |_|.|_|.|
            // |.|_|5|_|
            var board = Board("40 00 00 05");
            Action doMove = () => board.PerformNewMove(Pos(1, 0), Pos(1, 2), _settings, out bool canCaptureMore);
            doMove.Should().Throw<ManualValidationException>();
            board.ToLongString(" ", "").Should().Be("40 00 00 05");
        }

        [Fact]
        public void MovePiece() {
            // |_|4|_|.|
            // |.|_|.|_|
            // |_|.|_|.|
            // |.|_|5|_|
            var board = Board("40 00 00 05");
            board.PerformNewMove(Pos(1, 0), Pos(0, 1), _settings, out bool canCaptureMore);
            board.ToLongString(" ", "").Should().Be("00 40 00 05");
            canCaptureMore.Should().BeFalse();
        }

        [Fact]
        public void CapturePiece() {
            // |_|4|_|.|
            // |.|_|5|_|
            // |_|.|_|.|
            // |.|_|5|_|
            var board = Board("40 05 00 05");
            board.PerformNewMove(Pos(1, 0), Pos(3, 2), _settings, out bool canCaptureMore);
            board.ToLongString(" ", "").Should().Be("00 00 04 05");
            canCaptureMore.Should().BeFalse();
        }

        [Fact]
        public void InValidMoveInChainSequenceThrows() {
            // |_|4|_|.|
            // |.|_|5|_|
            // |_|4|_|.|
            // |.|_|5|_|
            var board = Board("40 05 40 05");
            Action doMove = () => board.PerformChainCaptureMove(Pos(1, 0), Pos(3, 0), _settings, out bool canCaptureMore);
            doMove.Should().Throw<ManualValidationException>();
            board.ToLongString(" ", "").Should().Be("40 05 40 05");
        }

        [Fact]
        public void ValidNonCaptureMoveInChainSequenceThrows() {
            // |_|4|_|.|
            // |.|_|5|_|
            // |_|4|_|.|
            // |.|_|5|_|
            var board = Board("40 05 40 05");
            Action doMove = () => board.PerformChainCaptureMove(Pos(1, 0), Pos(0, 1), _settings, out bool canCaptureMore);
            doMove.Should().Throw<ManualValidationException>();
            board.ToLongString(" ", "").Should().Be("40 05 40 05");
        }

        [Fact]
        public void CapturePieceInChainSequence() {
            // |_|4|_|.|
            // |.|_|5|_|
            // |_|4|_|.|
            // |.|_|5|_|
            var board = Board("40 05 40 05");
            board.PerformChainCaptureMove(Pos(1, 0), Pos(3, 2), _settings, out bool canCaptureMore);
            board.ToLongString(" ", "").Should().Be("00 00 44 05");
            canCaptureMore.Should().BeFalse();
        }

        [Fact]
        public void BlackManCanPromoteOnLastLine() {
            // |_|.|_|.|
            // |.|_|.|_|
            // |_|.|_|.|
            // |4|_|.|_|
            Board("00 00 00 40").CanPromote(Pos(0, 3)).Should().BeTrue();
        }

        [Fact]
        public void WhiteManCanPromoteOnFirstLine() {
            // |_|.|_|5|
            // |.|_|.|_|
            // |_|.|_|.|
            // |5|_|.|_|
            Board("05 00 00 00").CanPromote(Pos(3, 0)).Should().BeTrue();
        }

        [Fact]
        public void BlackManCantPromoteOnFirstLine() {
            // |_|4|_|.|
            // |.|_|.|_|
            // |_|.|_|.|
            // |.|_|.|_|
            Board("40 00 00 00").CanPromote(Pos(3, 0)).Should().BeFalse();
        }

        [Fact]
        public void KingCantPromote() {
            // |_|.|_|.|
            // |.|_|.|_|
            // |_|.|_|.|
            // |6|_|.|_|
            Board("00 00 00 60").CanPromote(Pos(0, 3)).Should().BeFalse();
        }

        [Fact]
        public void InvalidPromoteThrows() {
            // |_|.|_|.|
            // |.|_|.|_|
            // |_|4|_|.|
            // |.|_|.|_|
            var board = Board("00 00 40 00");
            Action promote = () => board.Promote(Pos(1, 2));
            promote.Should().Throw<ManualValidationException>();
            board.ToLongString(" ", "").Should().Be("00 00 40 00");
        }

        [Fact]
        public void PromotePiece() {
            // |_|.|_|.|
            // |.|_|.|_|
            // |_|.|_|.|
            // |4|_|.|_|
            var board = Board("00 00 00 40");
            board.Promote(Pos(0, 3));
            board.ToLongString(" ", "").Should().Be("00 00 00 60");
        }

        private BoardPosition Board(string board) => BoardPosition.FromString(board);

        private Square Pos(int x, int y) => Square.FromPosition(x, y, 4);
    }
}
