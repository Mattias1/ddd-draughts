using Draughts.Domain.AuthUserAggregate.Models;

namespace Draughts.Domain.AuthUserAggregate.Services {
    public interface IUserRoleDomainService {
        void AssignRole(AuthUser authUser, Role role);
        void RemoveRole(AuthUser authUser, Role role);
    }
}
