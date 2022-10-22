using Draughts.Domain.AuthContext.Models;
using Draughts.Repositories.Misc;
using NodaTime;
using System;

namespace Draughts.Domain.AuthContext.Services;

public sealed class UserRegistrationDomainService {
    private readonly IClock _clock;

    public UserRegistrationDomainService(IClock clock) {
        _clock = clock;
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

        authUser.RemoveRole(pendingRegistrationRole.Id, pendingRegistrationRole.Rolename);
        authUser.AssignRole(registeredUserRole.Id, registeredUserRole.Rolename);
    }
}
