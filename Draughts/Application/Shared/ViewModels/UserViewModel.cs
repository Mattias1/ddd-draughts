using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;

namespace Draughts.Application.Shared.ViewModels {
    public class UserViewModel {
        public UserId Id { get; }
        public AuthUserId AuthUserId { get; }
        public Username Username { get; }
        public Rating Rating { get; private set; }
        public Rank Rank { get; private set; }
        public int GamesPlayed { get; private set; }

        public UserViewModel(User user) {
            Id = user.Id;
            AuthUserId = user.AuthUserId;
            Username = user.Username;
            Rating = user.Rating;
            Rank = user.Rank;
            GamesPlayed = user.GamesPlayed;
        }
    }
}
