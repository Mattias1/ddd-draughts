using Draughts.Common;
using Draughts.Common.Utilities;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using NodaTime;
using NodaTime.Testing;
using System;
using Xunit;

namespace Draughts.Test.Domain.GameContext;

public sealed class GameModToolsTest {
    private const int TWO_DAYS = 2 * 24 * 60 * 60;
    private FakeClock _fakeClock = FakeClock.FromUtc(2020, 02, 29);

    [Fact]
    public void ChangingTurnTimeForFinishedGameThrowsValidationError() {
        var game = GameTestHelper.FinishedMiniGame().Build();

        Action changeTurnTime = () => game.ChangeTurnTime(_fakeClock.UtcNow(), TWO_DAYS, true);

        changeTurnTime.Should().Throw<ManualValidationException>();
    }

    [Fact]
    public void ChangingTurnTimeForAllTurns() {
        var game = GameTestHelper.StartedMiniGame().Build();

        game.ChangeTurnTime(_fakeClock.UtcNow(), TWO_DAYS, true);

        game.Turn?.ExpiresAt.Should().Be(_fakeClock.UtcNow().Plus(Duration.FromDays(2)));
        game.Settings.MaxTurnLength.Should().Be(Duration.FromDays(2));
    }

    [Fact]
    public void ChangingTurnTimeForThisTurnOnly() {
        var game = GameTestHelper.StartedMiniGame().Build();

        game.ChangeTurnTime(_fakeClock.UtcNow(), TWO_DAYS, false);

        game.Turn?.ExpiresAt.Should().Be(_fakeClock.UtcNow().Plus(Duration.FromDays(2)));
        game.Settings.MaxTurnLength.Should().Be(Duration.FromDays(1));
    }
}
