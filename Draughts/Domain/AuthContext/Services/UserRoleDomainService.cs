using Draughts.Common;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using System;
using System.Linq;

namespace Draughts.Domain.AuthContext.Services;

public sealed class UserRoleDomainService {
    public void AssignRole(AuthUser authUser, Role role, UserId? responsibleUserId) {
        AssertResponsibleUserIdIfNecessary(role, responsibleUserId);

        authUser.AssignRole(role.Id, role.Rolename, responsibleUserId);
    }

    public void RemoveRole(AuthUser authUser, Role role, UserId? responsibleUserId) {
        AssertResponsibleUserIdIfNecessary(role, responsibleUserId);

        if (role.Rolename == Role.ADMIN_ROLENAME && AuthUser.PROTECTED_USERS.Contains(authUser.Username.Value)) {
            throw new ManualValidationException("You can't remove the admin role from protected users.");
        }
        authUser.RemoveRole(role.Id, role.Rolename, responsibleUserId);
    }

    private static void AssertResponsibleUserIdIfNecessary(Role role, UserId? responsibleUserId) {
        if (responsibleUserId is null && !role.IsUserCreationRole()) {
            throw new InvalidOperationException("Assigning roles need to log the responsible user id.");
        }
    }
}
