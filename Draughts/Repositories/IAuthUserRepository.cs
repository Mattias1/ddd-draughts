using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;

namespace Draughts.Repositories {
    public interface IAuthUserRepository : IRepository<AuthUser, UserId> {
    }
}
