using Draughts.Domain.AuthUserContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories;

namespace Draughts.Application.Auth.Services {
    public interface IUserRegistrationService {
        AuthUser CreateAuthUser(string? name, string? email, string? plaintextPassword);
        AuthUser FinishRegistration(UserId id);
    }
}
