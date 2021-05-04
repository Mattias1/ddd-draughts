using Draughts.Domain.AuthUserContext.Models;
using Draughts.Domain.UserContext.Models;

namespace Draughts.Repositories {
    public interface IAuthUserRepository : IRepository<AuthUser, UserId> {
    }
}
