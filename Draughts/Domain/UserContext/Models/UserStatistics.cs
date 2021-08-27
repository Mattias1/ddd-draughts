using Draughts.Common.OoConcepts;

namespace Draughts.Domain.UserContext.Models {
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

        public static UserStatistics BuildNew(UserId id) {
            return new UserStatistics(id, GamesTally.Empty, GamesTally.Empty, GamesTally.Empty, GamesTally.Empty);
        }
    }

    public record GamesTally(int Played, int Won, int Tied, int Lost) {
        public static GamesTally Empty => new GamesTally(0, 0, 0, 0);
    }
}
