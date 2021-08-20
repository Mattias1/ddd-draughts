using Draughts.Common.OoConcepts;
using Draughts.Domain.AuthContext.Models;
using NodaTime;

namespace Draughts.Domain.UserContext.Models {
    public class User : Entity<User, UserId> {
        public override UserId Id { get; }
        public Username Username { get; }
        public Rating Rating { get; private set; }
        public Rank Rank { get; private set; }
        public int GamesPlayed { get; private set; }
        public ZonedDateTime CreatedAt { get; }

        public User(UserId id, Username username, Rating rating, Rank rank,
                int gamesPlayed, ZonedDateTime createdAt) {
            Id = id;
            Username = username;
            Rating = rating;
            Rank = rank;
            GamesPlayed = gamesPlayed;
            CreatedAt = createdAt;
        }
    }
}
