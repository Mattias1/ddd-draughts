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
    public void IsForwardsDirection(string color, string direction, bool result) {
        GetDirection(direction).IsForwardsDirection(Color.FromString(color)).Should().Be(result);
    }

    private Direction GetDirection(string direction) => direction switch {
        "nw" => Direction.NW,
        "ne" => Direction.NE,
        "se" => Direction.SE,
        "sw" => Direction.SW,
        _ => throw new InvalidOperationException()
    };
}
