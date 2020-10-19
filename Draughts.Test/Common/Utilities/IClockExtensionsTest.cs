using Draughts.Common.Utilities;
using FluentAssertions;
using NodaTime.Testing;
using Xunit;

namespace Draughts.Test.Common.Utilities {
    public class IClockExtensionsTest {
        [Fact]
        public void TestUtcNow() {
            var clock = FakeClock.FromUtc(2020, 02, 29, 23, 59, 18);

            var now = clock.UtcNow();

            now.Year.Should().Be(2020);
            now.Month.Should().Be(2);
            now.Day.Should().Be(29);
            now.Hour.Should().Be(23);
            now.Minute.Should().Be(59);
            now.Second.Should().Be(18);
        }
    }
}
