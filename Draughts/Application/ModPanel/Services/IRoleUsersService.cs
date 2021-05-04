using Draughts.Domain.AuthUserContext.Models;
using Draughts.Domain.UserContext.Models;
using System.Collections.Generic;

namespace Draughts.Application.ModPanel.Services {
    public interface IRoleUsersService {
        (Role role, IReadOnlyList<AuthUser> authUsers) GetRoleWithUsers(RoleId roleId);
        void AssignRole(UserId responsibleUserId, RoleId roleId, Username username);
        void RemoveRole(UserId responsibleUserId, RoleId roleId, UserId userId);
    }
}
