using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;

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
