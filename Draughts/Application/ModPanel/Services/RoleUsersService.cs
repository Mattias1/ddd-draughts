using Draughts.Common;
using Draughts.Domain.AuthContext.Events;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.AuthContext.Services;
using Draughts.Domain.AuthContext.Specifications;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using NodaTime;
using System.Collections.Generic;

namespace Draughts.Application.ModPanel.Services;

public sealed class RoleUsersService {
    private readonly IAuthUserRepository _authUserRepository;
    private readonly IClock _clock;
    private readonly IIdGenerator _idGenerator;
    private readonly IRoleRepository _roleRepository;
    private readonly UserRoleDomainService _userRoleDomainService;
    private readonly IUnitOfWork _unitOfWork;

    public RoleUsersService(IAuthUserRepository authUserRepository, IClock clock, IIdGenerator idGenerator,
            IRoleRepository roleRepository, UserRoleDomainService userRoleDomainService, IUnitOfWork unitOfWork) {
        _authUserRepository = authUserRepository;
        _clock = clock;
        _idGenerator = idGenerator;
        _roleRepository = roleRepository;
        _userRoleDomainService = userRoleDomainService;
        _unitOfWork = unitOfWork;
    }

    public (Role role, IReadOnlyList<AuthUser> authUsers) GetRoleWithUsers(RoleId roleId) {
        return _unitOfWork.WithAuthTransaction(tran => {
            var role = FindRole(roleId);
            var authUsers = _authUserRepository.List(new UsersWithRoleSpecification(roleId));
            return (role, authUsers);
        });
    }

    public void AssignRole(UserId responsibleUserId, RoleId roleId, Username username) {
        _unitOfWork.WithAuthTransaction(tran => {
            var role = FindRole(roleId);
            var authUser = _authUserRepository.FindOrNull(new UsernameSpecification(username));
            if (authUser is null) {
                throw new ManualValidationException("User not found");
            }

            _userRoleDomainService.AssignRole(authUser, role);
            _authUserRepository.Save(authUser);

            _unitOfWork.Raise(UserGainedRole.Factory(role, authUser, responsibleUserId));
        });
    }

    public void RemoveRole(UserId responsibleUserId, RoleId roleId, UserId userId) {
        _unitOfWork.WithAuthTransaction(tran => {
            var role = FindRole(roleId);
            var authUser = _authUserRepository.FindByIdOrNull(userId);
            if (authUser is null) {
                throw new ManualValidationException("User not found");
            }

            _userRoleDomainService.RemoveRole(authUser, role);
            _authUserRepository.Save(authUser);

            _unitOfWork.Raise(UserLostRole.Factory(role, authUser, responsibleUserId));
        });
    }

    private Role FindRole(RoleId roleId) {
        var role = _roleRepository.FindByIdOrNull(roleId);
        if (role is null) {
            throw new ManualValidationException("Role not found");
        }
        return role;
    }
}
