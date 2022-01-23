using Draughts.Common;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Events;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.AuthContext.Specifications;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Application.ModPanel.Services;

public class EditRoleService {
    private readonly IAuthUserRepository _authUserRepository;
    private readonly IClock _clock;
    private readonly IIdGenerator _idGenerator;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public EditRoleService(IAuthUserRepository authUserRepository, IClock clock, IIdGenerator idGenerator,
            IRoleRepository roleRepository, IUnitOfWork unitOfWork) {
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
            var nextId = _idGenerator.ReservePool().Next();
            var createdAt = _clock.UtcNow();
            var role = new Role(new RoleId(nextId), rolename, createdAt);

            _roleRepository.Save(role);

            _unitOfWork.Raise(RoleCreated.Factory(role, responsibleUserId));

            return role;
        });
    }

    public void EditRole(UserId responsibleUserId, RoleId roleId, string rolename, string[] grantedPermissions) {
        _unitOfWork.WithAuthTransaction(tran => {
            var role = FindRole(roleId);
            role.Edit(rolename, grantedPermissions.Select(p => new Permission(p)));

            _roleRepository.Save(role);

            _unitOfWork.Raise(RoleEdited.Factory(role, responsibleUserId));
        });
    }

    public void DeleteRole(UserId responsibleUserId, RoleId roleId) {
        _unitOfWork.WithAuthTransaction(tran => {
            var role = FindRole(roleId);
            long nrOfUsersWithRole = _authUserRepository.Count(new UsersWithRoleSpecification(roleId));
            if (nrOfUsersWithRole > 0) {
                throw new ManualValidationException("You cannot delete roles with users assigned.");
            }

            _roleRepository.Delete(roleId);

            _unitOfWork.Raise(RoleDeleted.Factory(role, responsibleUserId));
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
