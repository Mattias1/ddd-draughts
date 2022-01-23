using Draughts.Common;
using Draughts.Domain.GameContext.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using System;
using Xunit;

namespace Draughts.Test.Domain.GameContext;

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
    public void CoordinateToIdOn4x4Board(int x, int y, int n) {
        // |_|1|_|2|
        // |3|_|4|_|
        // |_|5|_|6|
        // |7|_|8|_|
        SquareId.FromPosition(x, y, 4).Should().Be(n.AsSquare());
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
    public void CoordinateToIdOn8x8Board(int x, int y, int n) {
        SquareId.FromPosition(x, y, 8).Should().Be(n.AsSquare());
    }

    [Fact]
    public void NonPlayableCoordinatesShouldThrow() {
        // |_|.|_|.|
        // |.|x|.|_|
        // |_|.|_|.|
        // |.|_|.|_|
        Action fromPosition = () => SquareId.FromPosition(1, 1, 4);
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
    public void SquareIdToCoordinateOn4x4Board(int n, int x, int y) {
        // |_|1|_|2|
        // |3|_|4|_|
        // |_|5|_|6|
        // |7|_|8|_|
        n.AsSquare().ToPosition(4).Should().Be((x, y));
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
    public void SquareIdToCoordinateOn8x8Board(int n, int x, int y) {
        n.AsSquare().ToPosition(8).Should().Be((x, y));
    }

    [Theory]
    [InlineData(6, 1, 2, 10, 9)]
    [InlineData(9, 5, 6, 14, 13)]
    [InlineData(4, null, null, null, 8)]
    [InlineData(29, null, 25, null, null)]
    public void BorderSquaresOn8x8Board(int n, int? nw, int? ne, int? se, int? sw) {
        var board = Board.InitialSetup(8);
        var square = board[n.AsSquare()];

        Square? result;
        square.TryGetBorder(Direction.NW, out result).Should().Be(nw is not null);
        result?.Id.Value.Should().Be(nw);

        square.TryGetBorder(Direction.NE, out result).Should().Be(ne is not null);
        result?.Id.Value.Should().Be(ne);

        square.TryGetBorder(Direction.SE, out result).Should().Be(se is not null);
        result?.Id.Value.Should().Be(se);

        square.TryGetBorder(Direction.SW, out result).Should().Be(sw is not null);
        result?.Id.Value.Should().Be(sw);
    }
}
