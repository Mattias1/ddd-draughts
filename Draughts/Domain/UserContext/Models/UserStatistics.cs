using Draughts.Common.OoConcepts;
using static Draughts.Domain.GameContext.Models.GameSettings;

namespace Draughts.Domain.UserContext.Models;

public sealed class UserStatistics : Entity<UserStatistics, UserId> {
    public override UserId Id { get; }
    public GamesTally TotalTally { get; private set; }
    public GamesTally InternationalTally { get; private set; }
    public GamesTally EnglishAmericanTally { get; private set; }
    public GamesTally HexdameTally { get; private set;  }
    public GamesTally OtherTally { get; private set; }

    public UserStatistics(UserId id, GamesTally totalTally, GamesTally internationalTally,
            GamesTally englishAmericanTally, GamesTally hexdameTally, GamesTally otherTally) {
        Id = id;
        TotalTally = totalTally;
        InternationalTally = internationalTally;
        EnglishAmericanTally = englishAmericanTally;
        HexdameTally = hexdameTally;
        OtherTally = otherTally;
    }

    public void UpdateForFinishedGame(GameSettingsPreset gameSettingsPreset, UserId? victor) {
        bool? isWonTiedOrLost = victor is null ? null : Id == victor;

        TotalTally = TotalTally.WithFinishedGame(isWonTiedOrLost);
        if (gameSettingsPreset == GameSettingsPreset.International) {
            InternationalTally = InternationalTally.WithFinishedGame(isWonTiedOrLost);
        } else if (gameSettingsPreset == GameSettingsPreset.EnglishAmerican) {
            EnglishAmericanTally = EnglishAmericanTally.WithFinishedGame(isWonTiedOrLost);
        } else if (gameSettingsPreset == GameSettingsPreset.Hexdame) {
            HexdameTally = HexdameTally.WithFinishedGame(isWonTiedOrLost);
        } else {
            OtherTally = OtherTally.WithFinishedGame(isWonTiedOrLost);
        }
    }

    public static UserStatistics BuildNew(UserId id) {
        return new UserStatistics(id, GamesTally.Empty, GamesTally.Empty, GamesTally.Empty,
            GamesTally.Empty, GamesTally.Empty);
    }
}

public sealed record GamesTally(int Played, int Won, int Tied, int Lost) {
    public GamesTally WithFinishedGame(bool? isWonTiedOrLost) {
        return new GamesTally(
            Played + 1,
            isWonTiedOrLost == true ? Won + 1 : Won,
            isWonTiedOrLost is null ? Tied + 1 : Tied,
            isWonTiedOrLost == false ? Lost + 1 : Lost
        );
    }

    public static GamesTally Empty => new GamesTally(0, 0, 0, 0);
}
