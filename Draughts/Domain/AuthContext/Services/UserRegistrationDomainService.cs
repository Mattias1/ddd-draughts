using Draughts.Common;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories;
using NodaTime;
using System;
using System.Linq;

namespace Draughts.Domain.AuthContext.Services;

public sealed class UserRegistrationDomainService {
    private readonly IClock _clock;
    private readonly UserRoleDomainService _userRoleService;

    public UserRegistrationDomainService(IClock clock, UserRoleDomainService userRoleService) {
        _clock = clock;
        _userRoleService = userRoleService;
    }

    public AuthUser CreateAuthUser(IIdPool idPool, Role pendingRegistrationRole,
            string? name, string? email, string? plaintextPassword) {
        if (pendingRegistrationRole.Rolename != Role.PENDING_REGISTRATION_ROLENAME) {
            throw new ArgumentException("You're supposed to add the pending registration role.",
                nameof(pendingRegistrationRole));
        }

        return AuthUser.CreateNew(idPool, new Username(name), new Email(email), plaintextPassword,
            pendingRegistrationRole.Id, _clock);
    }

    public void Register(AuthUser authUser, Role registeredUserRole, Role pendingRegistrationRole) {
        if (registeredUserRole.Rolename != Role.REGISTERED_USER_ROLENAME) {
            throw new ArgumentException("You're supposed to pass the registered user role.",
                nameof(registeredUserRole));
        }
        if (pendingRegistrationRole.Rolename != Role.PENDING_REGISTRATION_ROLENAME) {
            throw new ArgumentException("You're supposed to pass the pending registration role.",
                nameof(pendingRegistrationRole));
        }

        if (!authUser.RoleIds.Contains(pendingRegistrationRole.Id)) {
            throw new ManualValidationException("This user doesn't have the pending registration role.");
        }

        _userRoleService.RemoveRole(authUser, pendingRegistrationRole, null);
        _userRoleService.AssignRole(authUser, registeredUserRole, null);
    }
}
