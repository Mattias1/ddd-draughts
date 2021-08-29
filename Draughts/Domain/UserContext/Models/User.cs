using Draughts.Common.OoConcepts;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.GameContext.Models;
using NodaTime;

namespace Draughts.Domain.UserContext.Models {
    public class User : Entity<User, UserId> {
        public override UserId Id { get; }
        public Username Username { get; }
        public Rating Rating { get; private set; }
        public Rank Rank { get; private set; }
        public UserStatistics Statistics { get; }
        public ZonedDateTime CreatedAt { get; }

        public User(UserId id, Username username, Rating rating, Rank rank,
                UserStatistics statistics, ZonedDateTime createdAt) {
            Id = id;
            Username = username;
            Rating = rating;
            Rank = rank;
            Statistics = statistics;
            CreatedAt = createdAt;
        }

        public void UpdateStatisticsForFinishedGame(GameSettings.GameSettingsPreset settingsPreset, UserId? victor) {
            Statistics.UpdateForFinishedGame(settingsPreset, victor);
        }

        public static User BuildNew(UserId id, Username username, ZonedDateTime createdAt) {
            return new User(id, username, Rating.StartRating, Rank.Ranks.Private, UserStatistics.BuildNew(id), createdAt);
        }
    }
}
