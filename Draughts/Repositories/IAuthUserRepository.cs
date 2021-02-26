using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;

namespace Draughts.Repositories {
    public interface IAuthUserRepository : IRepository<AuthUser> {
        AuthUser FindById(UserId id);
        AuthUser? FindByIdOrNull(UserId id);
    }
}
