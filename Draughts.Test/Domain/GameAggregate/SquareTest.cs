using Draughts.Common;
using Draughts.Domain.GameAggregate.Models;
using FluentAssertions;
using System;
using Xunit;

namespace Draughts.Test.Domain.GameAggregate {
    public class SquareTest {
        [Theory]
        [InlineData(1, 0, 1)]
        [InlineData(3, 0, 2)]
        [InlineData(0, 1, 3)]
        [InlineData(2, 1, 4)]
        [InlineData(1, 2, 5)]
        [InlineData(3, 2, 6)]
        [InlineData(0, 3, 7)]
        [InlineData(2, 3, 8)]
        public void CoordinateToNumberOn4x4Board(int x, int y, int n) {
            /*|_|1|_|2|
              |3|_|4|_|
              |_|5|_|6|
              |7|_|8|_|*/
            Square.FromPosition(x, y, 4).Should().Be(new Square(n));
        }

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
            /*|_|1|_|2|
              |3|_|4|_|
              |_|5|_|6|
              |7|_|8|_|*/
            Square.FromPosition(x, y, 8).Should().Be(new Square(n));
        }

        [Fact]
        public void NonPlayableCoordinatesShouldThrow() {
            /*|_|.|_|.|
              |.|x|.|_|
              |_|.|_|.|
              |.|_|.|_|*/
            Action fromPosition = () => Square.FromPosition(1, 1, 4);
            fromPosition.Should().Throw<ManualValidationException>();
        }

        [Theory]
        [InlineData(1, 1, 0)]
        [InlineData(2, 3, 0)]
        [InlineData(3, 0, 1)]
        [InlineData(4, 2, 1)]
        [InlineData(5, 1, 2)]
        [InlineData(6, 3, 2)]
        [InlineData(7, 0, 3)]
        [InlineData(8, 2, 3)]
        public void NumberToCoordinateOn4x4Board(int n, int x, int y) {
            /*|_|1|_|2|
              |3|_|4|_|
              |_|5|_|6|
              |7|_|8|_|*/
            new Square(n).ToPosition(4).Should().Be((x, y));
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
            new Square(n).ToPosition(8).Should().Be((x, y));
        }

        [Theory]
        [InlineData(6, 1, 2, 10, 9)]
        [InlineData(9, 5, 6, 14, 13)]
        [InlineData(4, null, null, null, 8)]
        [InlineData(29, null, 25, null, null)]
        public void BorderSquaresOn8x8Board(int n, int? nw, int? ne, int? se, int? sw) {
            var nr = new Square(n);

            Square? result;
            nr.TryGetBorder(Direction.NW, 8, out result).Should().Be(nw != null);
            result?.Value.Should().Be(nw);

            nr.TryGetBorder(Direction.NE, 8, out result).Should().Be(ne != null);
            result?.Value.Should().Be(ne);

            nr.TryGetBorder(Direction.SE, 8, out result).Should().Be(se != null);
            result?.Value.Should().Be(se);

            nr.TryGetBorder(Direction.SW, 8, out result).Should().Be(sw != null);
            result?.Value.Should().Be(sw);
        }
    }
}
