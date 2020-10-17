using Draughts.Common;
using Draughts.Domain.GameAggregate.Models;
using FluentAssertions;
using System;
using Xunit;

namespace Draughts.Test.Domain.GameAggregate {
    public class SquareNumberTest {
        [Theory]
        [InlineData(1, 0, 1)]
        [InlineData(3, 0, 2)]
        [InlineData(5, 0, 3)]
        [InlineData(7, 0, 4)]
        [InlineData(0, 1, 5)]
        [InlineData(2, 1, 6)]
        [InlineData(6, 1, 8)]
        [InlineData(1, 2, 9)]
        [InlineData(2, 3, 14)]
        [InlineData(6, 7, 32)]
        public void CoordinateToNumberOn8x8Board(int x, int y, int n) {
            SquareNumber.FromPosition(x, y, 8).Should().Be(new SquareNumber(n));
        }

        [Fact]
        public void NonPlayableCoordinatesShouldThrow() {
            Action fromPosition = () => SquareNumber.FromPosition(1, 1, 8);
            fromPosition.Should().Throw<ManualValidationException>();
        }

        [Theory]
        [InlineData(1, 1, 0)]
        [InlineData(2, 3, 0)]
        [InlineData(3, 5, 0)]
        [InlineData(4, 7, 0)]
        [InlineData(5, 0, 1)]
        [InlineData(6, 2, 1)]
        [InlineData(8, 6, 1)]
        [InlineData(9, 1, 2)]
        [InlineData(14, 2, 3)]
        [InlineData(32, 6, 7)]
        public void NumberToCoordinateOn8x8Board(int n, int x, int y) {
            new SquareNumber(n).ToPosition(8).Should().Be((x, y));
        }
    }
}
