using Draughts.Common.Utilities;
using FluentAssertions;
using NodaTime;
using NodaTime.Testing;
using Xunit;

namespace Draughts.Test.Common.Utilities;

public sealed class NodaTimeExtensionsTest {
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

    [Fact]
    public void UtcToIsoStringReturnsOldskoolIsoValue() {
        var clock = FakeClock.FromUtc(2020, 02, 29, 23, 59, 18);
        clock.AdvanceMilliseconds(37);
        var now = clock.UtcNow();

        now.ToIsoString().Should().Be("2020-02-29T23:59:18Z");
    }

    [Fact]
    public void NullToIsoStringReturnsEmptyString() {
        ZonedDateTime? now = null;
        now.ToIsoString().Should().Be("");
    }
}
