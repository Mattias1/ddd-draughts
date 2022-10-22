using Draughts.Common.Events;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Events;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using NodaTime;
using NodaTime.Testing;
using Xunit;
using static Draughts.Domain.GameContext.Models.GameSettings;

namespace Draughts.Test.Common.Events;

public sealed class DomainEventTest {
    private static GameSettingsPreset Preset = GameSettingsPreset.International;
    private static UserId[] Players = new UserId[] { new UserId(1), new UserId(2) };

    private readonly IClock _clock;

    public DomainEventTest() {
        _clock = FakeClock.FromUtc(2020, 02, 29);
    }

    [Fact]
    public void EqualWhenIdsAreEqual() {
        var left = new GameFinished(new GameId(1), Players, null, Preset, new DomainEventId(1), _clock.UtcNow(), null);
        var right = new GameFinished(new GameId(2), Players, null, Preset, new DomainEventId(1), _clock.UtcNow(), null);

        left.Equals((object)right).Should().BeTrue();
        left.Equals(right).Should().BeTrue();
        (left == right).Should().BeTrue();
        (left != right).Should().BeFalse();
        left.GetHashCode().Should().Be(right.GetHashCode());
    }

    [Fact]
    public void NotEqualWhenIdsAreDifferent() {
        var role = RoleTestHelper.PendingRegistration().WithId(11).Build();
        var left = new GameFinished(new GameId(1), Players, null, Preset, new DomainEventId(1), _clock.UtcNow(), null);
        var right = new GameFinished(new GameId(1), Players, null, Preset, new DomainEventId(2), _clock.UtcNow(), null);

        left.Equals((object)right).Should().BeFalse();
        left.Equals(right).Should().BeFalse();
        (left == right).Should().BeFalse();
        (left != right).Should().BeTrue();
    }

    [Fact]
    public void NotEqualWhenTheOtherIsNull() {
        var role = RoleTestHelper.PendingRegistration().WithId(11).Build();
        var left = new GameFinished(new GameId(1), Players, null, Preset, new DomainEventId(1), _clock.UtcNow(), null);
        GameFinished? right = null;

        left.Equals(right as object).Should().BeFalse();
        left.Equals(right).Should().BeFalse();
        (left == right).Should().BeFalse();
        (left != right).Should().BeTrue();

        (right == left).Should().BeFalse();
        (right != left).Should().BeTrue();
    }

    [Fact]
    public void EqualWhenBothAreNull() {
        GameFinished? left = null;
        GameFinished? right = null;

        (left == right).Should().BeTrue();
        (left != right).Should().BeFalse();
    }
}
