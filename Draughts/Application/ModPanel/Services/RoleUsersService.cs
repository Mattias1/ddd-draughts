using Draughts.Common;
using Draughts.Domain.AuthUserAggregate.Events;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.AuthUserAggregate.Specifications;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using NodaTime;
using System.Collections.Generic;

namespace Draughts.Application.ModPanel.Services {
    public class RoleUsersService : IRoleUsersService {
        private readonly IAuthUserRepository _authUserRepository;
        private readonly IClock _clock;
        private readonly IIdGenerator _idGenerator;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RoleUsersService(IAuthUserRepository authUserRepository, IClock clock, IIdGenerator idGenerator,
                IRoleRepository roleRepository, IUnitOfWork unitOfWork) {
            _authUserRepository = authUserRepository;
            _clock = clock;
            _idGenerator = idGenerator;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        public (Role role, IReadOnlyList<AuthUser> authUsers) GetRoleWithUsers(RoleId roleId) {
            return _unitOfWork.WithAuthUserTransaction(tran => {
                var role = FindRole(roleId);
                var authUsers = _authUserRepository.List(new UsersWithRoleSpecification(roleId));
                return tran.CommitWith((role, authUsers));
            });
        }

        public void AssignRole(UserId responsibleUserId, RoleId roleId, Username username) {
            _unitOfWork.WithAuthUserTransaction(tran => {
                var role = FindRole(roleId);
                var authUser = _authUserRepository.FindOrNull(new UsernameSpecification(username));
                if (authUser is null) {
                    throw new ManualValidationException("User not found");
                }

                authUser.AssignRole(role);
                _authUserRepository.Save(authUser);

                _unitOfWork.Raise(UserGainedRole.Factory(role, authUser, responsibleUserId));

                tran.Commit();
            });
        }

        public void RemoveRole(UserId responsibleUserId, RoleId roleId, UserId userId) {
            _unitOfWork.WithAuthUserTransaction(tran => {
                var role = FindRole(roleId);
                var authUser = _authUserRepository.FindByIdOrNull(userId);
                if (authUser is null) {
                    throw new ManualValidationException("User not found");
                }

                authUser.RemoveRole(role);
                _authUserRepository.Save(authUser);

                _unitOfWork.Raise(UserLostRole.Factory(role, authUser, responsibleUserId));

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
