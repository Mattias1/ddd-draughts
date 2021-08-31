using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.AuthContext.Specifications;
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

        public AuthUser FindByName(string username) => Find(new UsernameSpecification(username));

        protected override IList<AuthUser> GetBaseQuery() {
            var authUserRoles = AuthDatabase.Get.AuthUserRolesTable.ToLookup(ar => ar.UserId, ar => new RoleId(ar.RoleId));
            return AuthDatabase.Get.AuthUsersTable
                .Select(u => u.ToDomainModel(authUserRoles[u.Id].ToList().AsReadOnly()))
                .ToList();
        }

        public override void Save(AuthUser entity) {
            var dbAuthUser = DbAuthUser.FromDomainModel(entity);
            _unitOfWork.Store(dbAuthUser, tran => AuthDatabase.Temp(tran).AuthUsersTable);

            foreach (var roleId in entity.RoleIds.Select(r => r.Id)) {
                var dbAuthUserRole = new DbAuthUserRole {
                    UserId = dbAuthUser.Id,
                    RoleId = roleId
                };
                _unitOfWork.Store(dbAuthUserRole, tran => AuthDatabase.Temp(tran).AuthUserRolesTable);
            }
        }
    }
}
