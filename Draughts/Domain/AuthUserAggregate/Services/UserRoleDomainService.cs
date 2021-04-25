using Draughts.Common;
using Draughts.Domain.AuthUserAggregate.Models;
using System;
using System.Linq;

namespace Draughts.Domain.AuthUserAggregate.Services {
    public class UserRoleDomainService : IUserRoleDomainService {
        public void AssignRole(AuthUser authUser, Role role) {
            authUser.AssignRole(role.Id);
        }

        public void RemoveRole(AuthUser authUser, Role role) {
            if (role.Rolename == Role.ADMIN_ROLENAME && AuthUser.PROTECTED_USERS.Contains(authUser.Username)) {
                throw new ManualValidationException("You can't remove the admin role from protected users.");
            }
            authUser.RemoveRole(role.Id);
        }
    }
}