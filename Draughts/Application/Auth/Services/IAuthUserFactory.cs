using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;

namespace Draughts.Application.Auth.Services {
    public interface IAuthUserFactory {
        AuthUser CreateAuthUser(IIdPool idPool, string? name, string? email, string? plaintextPassword);
        AuthUser FinishRegistration(UserId id);
    }
}
