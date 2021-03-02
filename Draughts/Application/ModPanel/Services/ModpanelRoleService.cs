using Draughts.Common;
using Draughts.Common.Events;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthUserAggregate.Events;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Application.ModPanel.Services {
    public class ModpanelRoleService : IModpanelRoleService {
        private readonly IClock _clock;
        private readonly IIdGenerator _idGenerator;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ModpanelRoleService(IClock clock, IIdGenerator idGenerator, IRoleRepository roleRepository,
                IUnitOfWork unitOfWork) {
            _clock = clock;
            _idGenerator = idGenerator;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public Role GetRole(RoleId roleId) {
            return _unitOfWork.WithAuthUserTransaction(tran => {
                var role = FindRole(roleId);
                return tran.CommitWith(role);
            });
        }

        public IReadOnlyList<Role> GetRoles() {
            return _unitOfWork.WithAuthUserTransaction(tran => {
                var roles = _roleRepository.List();
                return tran.CommitWith(roles);
            });
        }

        public void EditRole(UserId responsibleUserId, RoleId roleId, string rolename, string[] grantedPermissions) {
            _unitOfWork.WithAuthUserTransaction(tran => {
                var role = FindRole(roleId);
                role.Edit(rolename, grantedPermissions.Select(p => new Permission(p)));

                _roleRepository.Save(role);

                _unitOfWork.Raise(RoleEdited.Factory(role, responsibleUserId));

                tran.Commit();
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
}
