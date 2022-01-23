using Draughts.Domain.AuthContext.Models;
using Draughts.Repositories;

namespace Draughts.Domain.AuthContext.Services;

public interface IUserRegistrationDomainService {
    AuthUser CreateAuthUser(IIdPool idPool, Role pendingRegistrationRole, string? name, string? email, string? plaintextPassword);
    void Register(AuthUser authUser, Role registeredUserRole, Role pendingRegistrationRole);
}
