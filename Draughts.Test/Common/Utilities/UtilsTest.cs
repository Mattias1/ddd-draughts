using Draughts.Common.Utilities;
using Draughts.Application.ViewModels;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodaTime.Testing;

namespace Draughts.Test.Common.Utilities {
    [TestClass]
    public class UtilsTest {
        [TestMethod]
        [DataRow(2020, 02, 29, 13, 37, "29 Feb 2020, 13:37")]
        public void DateTime(int year, int month, int day, int hour, int min, string expected) {
            var clock = FakeClock.FromUtc(year, month, day, hour, min, 0);

            string result = Utils.DateTime(clock.UtcNow());

            result.Should().Be(expected);
        }
    }
}
