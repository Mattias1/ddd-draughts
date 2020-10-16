using Draughts.Common;
using Draughts.Domain.GameAggregate.Models;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Draughts.Test.Domain.GameAggregate {
    [TestClass]
    public class SquareNumberTest {
        [TestMethod]
        [DataRow(1, 0, 1)]
        [DataRow(3, 0, 2)]
        [DataRow(5, 0, 3)]
        [DataRow(7, 0, 4)]
        [DataRow(0, 1, 5)]
        [DataRow(2, 1, 6)]
        [DataRow(6, 1, 8)]
        [DataRow(1, 2, 9)]
        [DataRow(2, 3, 14)]
        [DataRow(6, 7, 32)]
        public void CoordinateToNumberOn8x8Board(int x, int y, int n) {
            SquareNumber.FromPosition(x, y, 8).Should().Be(new SquareNumber(n));
        }

        [TestMethod]
        public void NonPlayableCoordinatesShouldThrow() {
            Action fromPosition = () => SquareNumber.FromPosition(1, 1, 8);
            fromPosition.Should().Throw<ManualValidationException>();
        }

        [TestMethod]
        [DataRow(1, 1, 0)]
        [DataRow(2, 3, 0)]
        [DataRow(3, 5, 0)]
        [DataRow(4, 7, 0)]
        [DataRow(5, 0, 1)]
        [DataRow(6, 2, 1)]
        [DataRow(8, 6, 1)]
        [DataRow(9, 1, 2)]
        [DataRow(14, 2, 3)]
        [DataRow(32, 6, 7)]
        public void NumberToCoordinateOn8x8Board(int n, int x, int y) {
            new SquareNumber(n).ToPosition(8).Should().Be((x, y));
        }
    }
}
