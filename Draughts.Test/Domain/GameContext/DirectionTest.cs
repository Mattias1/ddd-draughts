using Draughts.Domain.GameContext.Models;
using FluentAssertions;
using System;
using Xunit;

namespace Draughts.Test.Domain.GameContext;

public sealed class DirectionTest {
    [Theory]
    [InlineData("black", "nw", false)]
    [InlineData("black", "ne", false)]
    [InlineData("black", "se", true)]
    [InlineData("black", "sw", true)]
    [InlineData("white", "nw", true)]
    [InlineData("white", "ne", true)]
    [InlineData("white", "se", false)]
    [InlineData("white", "sw", false)]
    public void IsForwardsSquareDirection(string color, string direction, bool result) {
        var dir = direction switch {
            "nw" => Direction.SQUARE_NW,
            "ne" => Direction.SQUARE_NE,
            "se" => Direction.SQUARE_SE,
            "sw" => Direction.SQUARE_SW,
            _ => throw new InvalidOperationException()
        };
        dir.IsForwardsDirection(Color.FromString(color)).Should().Be(result);
    }

    [Theory]
    [InlineData("black", "nw", false)]
    [InlineData("black", "n", false)]
    [InlineData("black", "ne", false)]
    [InlineData("black", "se", true)]
    [InlineData("black", "s", true)]
    [InlineData("black", "sw", true)]
    [InlineData("white", "nw", true)]
    [InlineData("white", "n", true)]
    [InlineData("white", "ne", true)]
    [InlineData("white", "se", false)]
    [InlineData("white", "s", false)]
    [InlineData("white", "sw", false)]
    public void IsForwardsHexDirection(string color, string direction, bool result) {
        var dir = direction switch {
            "n" => Direction.HEX_N,
            "ne" => Direction.HEX_NE,
            "se" => Direction.HEX_SE,
            "s" => Direction.HEX_S,
            "sw" => Direction.HEX_SW,
            "nw" => Direction.HEX_NW,
            _ => throw new InvalidOperationException()
        };
        dir.IsForwardsDirection(Color.FromString(color)).Should().Be(result);
    }
}
