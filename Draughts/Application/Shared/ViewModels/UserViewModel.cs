using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;

namespace Draughts.Application.Shared.ViewModels;

public class BasicUserViewModel {
    public UserId Id { get; }
    public Username Username { get; }

    public BasicUserViewModel(UserId id, Username username) {
        Id = id;
        Username = username;
    }
}

public sealed class UserViewModel : BasicUserViewModel {
    public Rating Rating { get; }
    public Rank Rank { get; }
    public UserStatistics Statistics { get; }

    public UserViewModel(User user) : base(user.Id, user.Username) {
        Rating = user.Rating;
        Rank = user.Rank;
        Statistics = user.Statistics;
    }
}
