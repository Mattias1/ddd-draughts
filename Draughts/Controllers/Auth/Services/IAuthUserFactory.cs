using Draughts.Domain.AuthUserAggregate.Models;

namespace Draughts.Controllers.Auth.Services {
    public interface IAuthUserFactory {
        AuthUser CreateAuthUser(string? name, string? email, string? plaintextPassword);
        AuthUser FinishRegistration(AuthUserId id);
    }
}