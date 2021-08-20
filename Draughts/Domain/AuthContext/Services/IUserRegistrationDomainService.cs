using Draughts.Domain.AuthUserContext.Models;
using Draughts.Repositories;

namespace Draughts.Domain.AuthUserContext.Services {
    public interface IUserRegistrationDomainService {
        AuthUser CreateAuthUser(IIdPool idPool, Role pendingRegistrationRole, string? name, string? email, string? plaintextPassword);
        void Register(AuthUser authUser, Role registeredUserRole, Role pendingRegistrationRole);
    }
}
