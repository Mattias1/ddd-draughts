using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;

namespace Draughts.Application.Shared.ViewModels {
    public class BasicUserViewModel {
        public UserId Id { get; }
        public Username Username { get; }

        public BasicUserViewModel(UserId id, Username username) {
            Id = id;
            Username = username;
        }
    }

    public class UserViewModel : BasicUserViewModel {
        public Rating Rating { get; private set; }
        public Rank Rank { get; private set; }
        public int GamesPlayed { get; private set; }

        public UserViewModel(User user) : base(user.Id, user.Username) {
            Rating = user.Rating;
            Rank = user.Rank;
            GamesPlayed = user.GamesPlayed;
        }
    }
}
