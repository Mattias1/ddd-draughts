using Draughts.Common;
using Draughts.Domain.AuthUserAggregate.Models;

namespace Draughts.Domain.UserAggregate.Models {
    public class User {
        public UserId Id { get; }
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

        public override bool Equals(object? obj) => Equals(obj as User);
        public bool Equals(User? other) => other is null ? false : other.Id == Id;
        public override int GetHashCode() => Id.GetHashCode();
        public static bool operator ==(User? left, User? right) => Compare.NullSafeEquals(left, right);
        public static bool operator !=(User? left, User? right) => Compare.NullSafeNotEquals(left, right);
    }
}
