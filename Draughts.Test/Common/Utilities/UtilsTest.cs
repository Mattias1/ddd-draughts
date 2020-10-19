using Draughts.Application.Shared;
using Draughts.Common.Utilities;
using FluentAssertions;
using NodaTime.Testing;
using Xunit;

namespace Draughts.Test.Common.Utilities {
    public class UtilsTest {
        [Theory]
        [InlineData(2020, 02, 29, 13, 37, "29 Feb 2020, 13:37")]
        public void DateTimeParsing(int year, int month, int day, int hour, int min, string expected) {
            var clock = FakeClock.FromUtc(year, month, day, hour, min, 0);

            string result = Utils.DateTime(clock.UtcNow());

            result.Should().Be(expected);
        }
    }
}
