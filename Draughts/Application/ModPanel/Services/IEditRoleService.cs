using Draughts.Domain.AuthUserContext.Models;
using Draughts.Domain.UserContext.Models;
using System.Collections.Generic;

namespace Draughts.Application.ModPanel.Services {
    public interface IEditRoleService {
        Role CreateRole(UserId responsibleUserId, string rolename);
        void DeleteRole(UserId responsibleUserId, RoleId roleId);
        void EditRole(UserId responsibleUserId, RoleId roleId, string rolename, string[] permissions);
        Role GetRole(RoleId roleId);
        IReadOnlyList<Role> GetRoles();
    }
}
