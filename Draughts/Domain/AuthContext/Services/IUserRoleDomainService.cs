using Draughts.Domain.AuthContext.Models;

namespace Draughts.Domain.AuthContext.Services {
    public interface IUserRoleDomainService {
        void AssignRole(AuthUser authUser, Role role);
        void RemoveRole(AuthUser authUser, Role role);
    }
}
