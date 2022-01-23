using Draughts.Domain.UserContext.Models;
using FluentAssertions;
using Xunit;
using static Draughts.Domain.GameContext.Models.GameSettings;

namespace Draughts.Test.Domain.UserContext;

public class UserStatisticsTest {
    private UserId VictorId = new UserId(42);
    private UserId LoserId = new UserId(37);
    private UserId TiedPlayerId = new UserId(1);

    [Fact]
    public void UpdateStatisticsForInternationalVictory() {
        var userStats = UserStatistics.BuildNew(VictorId);

        userStats.UpdateForFinishedGame(GameSettingsPreset.International, VictorId);

        userStats.TotalTally.Should().Be(new GamesTally(1, 1, 0, 0));
        userStats.InternationalTally.Should().Be(new GamesTally(1, 1, 0, 0));
        userStats.EnglishAmericanTally.Should().Be(new GamesTally(0, 0, 0, 0));
        userStats.OtherTally.Should().Be(new GamesTally(0, 0, 0, 0));
    }

    [Fact]
    public void UpdateStatisticsForEnglishAmericanLoss() {
        var userStats = UserStatistics.BuildNew(LoserId);

        userStats.UpdateForFinishedGame(GameSettingsPreset.EnglishAmerican, VictorId);

        userStats.TotalTally.Should().Be(new GamesTally(1, 0, 0, 1));
        userStats.InternationalTally.Should().Be(new GamesTally(0, 0, 0, 0));
        userStats.EnglishAmericanTally.Should().Be(new GamesTally(1, 0, 0, 1));
        userStats.OtherTally.Should().Be(new GamesTally(0, 0, 0, 0));
    }

    [Fact]
    public void UpdateStatisticsForOtherDraw() {
        var userStats = UserStatistics.BuildNew(TiedPlayerId);

        userStats.UpdateForFinishedGame(GameSettingsPreset.Other, null);

        userStats.TotalTally.Should().Be(new GamesTally(1, 0, 1, 0));
        userStats.InternationalTally.Should().Be(new GamesTally(0, 0, 0, 0));
        userStats.EnglishAmericanTally.Should().Be(new GamesTally(0, 0, 0, 0));
        userStats.OtherTally.Should().Be(new GamesTally(1, 0, 1, 0));
    }

    [Fact]
    public void UpdateOtherStatisticsForMiniVictory() {
        var userStats = UserStatistics.BuildNew(VictorId);

        userStats.UpdateForFinishedGame(GameSettingsPreset.Mini, VictorId);

        userStats.TotalTally.Should().Be(new GamesTally(1, 1, 0, 0));
        userStats.InternationalTally.Should().Be(new GamesTally(0, 0, 0, 0));
        userStats.EnglishAmericanTally.Should().Be(new GamesTally(0, 0, 0, 0));
        userStats.OtherTally.Should().Be(new GamesTally(1, 1, 0, 0));
    }
}
