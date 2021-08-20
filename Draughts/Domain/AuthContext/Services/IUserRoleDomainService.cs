using Draughts.Domain.AuthUserContext.Models;

namespace Draughts.Domain.AuthUserContext.Services {
    public interface IUserRoleDomainService {
        void AssignRole(AuthUser authUser, Role role);
        void RemoveRole(AuthUser authUser, Role role);
    }
}
