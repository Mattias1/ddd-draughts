using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;

namespace Draughts.Application.Auth.Services {
    public interface IUserFactory {
        User CreateUser(AuthUserId authUserId, UserId userId, Username username);
    }
}