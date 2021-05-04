using Draughts.Domain.AuthUserContext.Models;
using Draughts.Domain.UserContext.Models;

namespace Draughts.Application.Auth.Services {
    public interface IUserFactory {
        User CreateUser(UserId userId, Username username);
    }
}
