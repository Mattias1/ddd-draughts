using Draughts.Domain.GameAggregate.Models;
using FluentAssertions;
using System;
using Xunit;

namespace Draughts.Test.Domain.GameAggregate {
    public class DirectionTest {
        [Theory]
        [InlineData("black", 1)]
        [InlineData("white", -1)]
        public void ForwardsYPosition(string color, int dy) {
            Direction.ForwardsYDirection(Color.FromString(color)).Should().Be(dy);
        }

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
}