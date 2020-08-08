using Draughts.Common.OoConcepts;
using Draughts.Domain.AuthUserAggregate.Models;

namespace Draughts.Domain.UserAggregate.Models {
    public class User : Entity<User, UserId> {
        public override UserId Id { get; }
        public AuthUserId AuthUserId { get; }
        public Username Username { get; }
        public Rating Rating { get; private set; }
        public Rank Rank { get; private set; }
        public int GamesPlayed { get; private set; }

        public User(UserId id, AuthUserId authUserId, Username username, Rating rating, Rank rank, int gamesPlayed) {
            Id = id;
            AuthUserId = authUserId;
            Username = username;
            Rating = rating;
            Rank = rank;
            GamesPlayed = gamesPlayed;
        }
    }
}
