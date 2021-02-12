using Draughts.Domain.GameAggregate.Models;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Draughts.Domain.GameAggregate.Models.BoardPosition;

namespace Draughts.Test.Domain.GameAggregate {
    public class PossibleMoveCalculatorTest {
        // TODO: This assumes man can capture backwards and more of this international sillyness ;)
        [Fact]
        public void ManMoves() {
            // |_|4|_|.|
            // |.|_|.|_|
            // |_|5|_|.|
            // |.|_|5|_|
            CalculatePossibleMoves("40005005", Color.White).Should().BeEquivalentTo("5-3", "5-4", "8-6");
        }

        [Fact]
        public void ManCaptures() {
            // |_|.|_|4|
            // |.|_|5|_|
            // |_|.|_|4|
            // |.|_|5|_|
            CalculatePossibleMoves("04050405", Color.Black).Should().BeEquivalentTo("2x5", "6x1");
        }

        [Fact]
        public void NoMovesWhenCapturing() {
            // |_|4|_|.|
            // |.|_|4|_|
            // |_|5|_|4|
            // |.|_|.|_|
            CalculatePossibleMoves("40045400", Color.Black).Should().BeEquivalentTo("4x7");
        }

        [Fact]
        public void KingMoves() {
            // |_|7|_|.|_|4|
            // |.|_|.|_|.|_|
            // |_|.|_|6|_|.|
            // |6|_|.|_|5|_|
            // |_|4|_|.|_|4|
            // |6|_|.|_|5|_|
            CalculatePossibleMoves("704 000 060 605 404 605", Color.Black)
                .Should().BeEquivalentTo("3-6", "8-5", "8-6", "8-11", "10-7", "10-5", "10-2", "13-17");
        }

        [Fact]
        public void KingCaptures() {
            // |_|.|_|.|_|.|
            // |.|_|5|_|.|_|
            // |_|6|_|.|_|4|
            // |.|_|4|_|7|_|
            // |_|.|_|4|_|.|
            // |7|_|.|_|.|_|
            CalculatePossibleMoves("000 050 604 047 040 700", Color.White)
                .Should().BeEquivalentTo("5x10", "12x17", "16x8", "16x6", "16x3");
        }

        [Fact]
        public void ChainCaptures() {
            // |_|.|_|.|_|.|
            // |.|_|4|_|.|_|
            // |_|.|_|.|_|.|
            // |.|_|4|_|4|_|
            // |_|.|_|5|_|.|
            // |.|_|.|_|.|_|
            CalculatePossibleMoves("000 040 000 044 050 000", Color.White)
                .Should().BeEquivalentTo("14x7");
        }

        [Fact]
        public void LongestChainCapturesMan() {
            // |_|.|_|.|_|.|
            // |5|_|.|_|.|_|
            // |_|4|_|4|_|.|
            // |.|_|.|_|.|_|
            // |_|4|_|4|_|.|
            // |.|_|5|_|.|_|
            CalculatePossibleMoves("000 500 440 000 440 050", Color.White)
                .Should().BeEquivalentTo("17x10", "17x12");
        }

        [Fact]
        public void LongestChainCapturesKing() {
            // |_|.|_|.|_|7|
            // |.|_|4|_|4|_|
            // |_|.|_|.|_|.|
            // |.|_|.|_|.|_|
            // |_|4|_|4|_|.|
            // |.|_|7|_|.|_|
            CalculatePossibleMoves("007 044 000 000 440 070", Color.White)
                .Should().BeEquivalentTo("17x10", "17x9");
        }

        private static List<string> CalculatePossibleMoves(string boardString, Color color) {
            var board = BoardPosition.FromString(boardString);
            return PossibleMoveCalculator.ForNewTurn(board, color)
                .Calculate()
                .Select(m => m.ToString())
                .ToList();
        }

        [Fact]
        public void OnlyCaptureFromRestrictedFieldWhenChaining() {
            // |_|.|_|.|
            // |.|_|5|_|
            // |_|4|_|4|
            // |.|_|5|_|
            var board = BoardPosition.FromString("00054405");
            var posisbleMoves = PossibleMoveCalculator.ForChainCaptures(board, new Square(5))
                .Calculate()
                .Select(m => m.ToString())
                .ToList();
            posisbleMoves.Should().BeEquivalentTo("5x2");
        }
    }
}