using Draughts.Common;
using Draughts.Domain.AuthContext.Events;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.AuthContext.Specifications;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories;
using Draughts.Repositories.Misc;
using Draughts.Repositories.Transaction;
using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Application.ModPanel.Services;

public sealed class EditRoleService {
    private readonly AuthUserRepository _authUserRepository;
    private readonly IClock _clock;
    private readonly IIdGenerator _idGenerator;
    private readonly RoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EditRoleService(AuthUserRepository authUserRepository, IClock clock, IIdGenerator idGenerator,
            RoleRepository roleRepository, IUnitOfWork unitOfWork) {
        _authUserRepository = authUserRepository;
        _clock = clock;
        _idGenerator = idGenerator;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    public Role GetRole(RoleId roleId) {
        return _unitOfWork.WithAuthTransaction(tran => {
            return FindRole(roleId);
        });
    }

    public IReadOnlyList<Role> GetRoles() {
        return _unitOfWork.WithAuthTransaction(tran => {
            return _roleRepository.List();
        });
    }

    public Role CreateRole(UserId responsibleUserId, string rolename) {
        return _unitOfWork.WithAuthTransaction(tran => {
            var role = Role.CreateNew(_idGenerator.ReservePool(), rolename, _clock, responsibleUserId);
            _roleRepository.Save(role);

            return role;
        });
    }

    public void EditRole(UserId responsibleUserId, RoleId roleId, string rolename, string[] grantedPermissions) {
        _unitOfWork.WithAuthTransaction(tran => {
            var role = FindRole(roleId);
            role.Edit(rolename, grantedPermissions.Select(p => new Permission(p)), responsibleUserId);

            _roleRepository.Save(role);
        });
    }

    public void DeleteRole(UserId responsibleUserId, RoleId roleId) {
        _unitOfWork.WithAuthTransaction(tran => {
            var role = FindRole(roleId);
            long nrOfUsersWithRole = _authUserRepository.Count(new UsersWithRoleSpecification(roleId));
            if (nrOfUsersWithRole > 0) {
                throw new ManualValidationException("You cannot delete roles with users assigned.");
            }

            _roleRepository.Delete(roleId, RoleDeleted.Factory(role, responsibleUserId));
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
