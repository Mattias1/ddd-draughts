using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;

namespace Draughts.Application.Auth.Services {
    public interface IUserRegistrationService {
        AuthUser CreateAuthUser(string? name, string? email, string? plaintextPassword);
        AuthUser FinishRegistration(UserId id);
    }
}
