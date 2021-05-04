using Draughts.Domain.AuthUserContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.InMemory {
    public class InMemoryAuthUserRepository : InMemoryRepository<AuthUser, UserId>, IAuthUserRepository {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InMemoryAuthUserRepository(IRoleRepository roleRepository, IUnitOfWork unitOfWork) {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        protected override IList<AuthUser> GetBaseQuery() {
            var authuserRoles = AuthUserDatabase.Get.AuthUserRolesTable.ToLookup(ar => ar.UserId, ar => new RoleId(ar.RoleId));
            return AuthUserDatabase.Get.AuthUsersTable
                .Select(u => u.ToDomainModel(authuserRoles[u.Id].ToList().AsReadOnly()))
                .ToList();
        }

        public override void Save(AuthUser entity) {
            var dbAuthUser = DbAuthUser.FromDomainModel(entity);
            _unitOfWork.Store(dbAuthUser, tran => AuthUserDatabase.Temp(tran).AuthUsersTable);

            foreach (var roleId in entity.RoleIds.Select(r => r.Id)) {
                var dbAuthUserRole = new DbAuthUserRole {
                    UserId = dbAuthUser.Id,
                    RoleId = roleId
                };
                _unitOfWork.Store(dbAuthUserRole, tran => AuthUserDatabase.Temp(tran).AuthUserRolesTable);
            }
        }
    }
}
