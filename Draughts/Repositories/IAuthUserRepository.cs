using Draughts.Domain.AuthUserAggregate.Models;

namespace Draughts.Repositories {
    public interface IAuthUserRepository : IRepository<AuthUser> {
        AuthUser FindById(AuthUserId id);
        AuthUser? FindByIdOrNull(AuthUserId id);
    }
}
