using Draughts.Common.OoConcepts;
using static Draughts.Domain.GameContext.Models.GameSettings;

namespace Draughts.Domain.UserContext.Models;

public class UserStatistics : Entity<UserStatistics, UserId> {
    public override UserId Id { get; }
    public GamesTally TotalTally { get; private set; }
    public GamesTally InternationalTally { get; private set; }
    public GamesTally EnglishAmericanTally { get; private set; }
    public GamesTally OtherTally { get; private set; }

    public UserStatistics(UserId id, GamesTally totalTally, GamesTally internationalTally,
            GamesTally englishAmericanTally, GamesTally otherTally) {
        Id = id;
        TotalTally = totalTally;
        InternationalTally = internationalTally;
        EnglishAmericanTally = englishAmericanTally;
        OtherTally = otherTally;
    }

    public void UpdateForFinishedGame(GameSettingsPreset gameSettingsPreset, UserId? victor) {
        bool? isWin = victor is null ? null : Id == victor;

        TotalTally = TotalTally.WithFinishedGame(isWin);
        if (gameSettingsPreset == GameSettingsPreset.International) {
            InternationalTally = InternationalTally.WithFinishedGame(isWin);
        }
        else if (gameSettingsPreset == GameSettingsPreset.EnglishAmerican) {
            EnglishAmericanTally = EnglishAmericanTally.WithFinishedGame(isWin);
        }
        else {
            OtherTally = OtherTally.WithFinishedGame(isWin);
        }
    }

    public static UserStatistics BuildNew(UserId id) {
        return new UserStatistics(id, GamesTally.Empty, GamesTally.Empty, GamesTally.Empty, GamesTally.Empty);
    }
}

public record GamesTally(int Played, int Won, int Tied, int Lost) {
    public GamesTally WithFinishedGame(bool? isWin) {
        return new GamesTally(
            Played + 1,
            isWin == true ? Won + 1 : Won,
            isWin == null ? Tied + 1 : Tied,
            isWin == false ? Lost + 1 : Lost
        );
    }

    public static GamesTally Empty => new GamesTally(0, 0, 0, 0);
}
