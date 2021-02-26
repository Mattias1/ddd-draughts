using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.AuthUserAggregate.Specifications;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.InMemory {
    public class InMemoryAuthUserRepository : InMemoryRepository<AuthUser>, IAuthUserRepository {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InMemoryAuthUserRepository(IRoleRepository roleRepository, IUnitOfWork unitOfWork) {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        protected override IList<AuthUser> GetBaseQuery() {
            var roles = _roleRepository.List().ToDictionary(r => r.Id.Id);
            var authuserRoles = AuthUserDatabase.AuthUserRolesTable.ToLookup(ar => ar.UserId, ar => roles[ar.RoleId]);
            return AuthUserDatabase.AuthUsersTable
                .Select(u => u.ToDomainModel(authuserRoles[u.Id].ToList().AsReadOnly()))
                .ToList();
        }

        public AuthUser FindById(UserId id) => Find(new AuthUserIdSpecification(id));
        public AuthUser? FindByIdOrNull(UserId id) => FindOrNull(new AuthUserIdSpecification(id));

        public override void Save(AuthUser entity) {
            var dbAuthUser = DbAuthUser.FromDomainModel(entity);
            _unitOfWork.Store(dbAuthUser, AuthUserDatabase.TempAuthUsersTable);

            foreach (var roleId in entity.Roles.Select(r => r.Id)) {
                var dbAuthUserRole = new DbAuthUserRole {
                    UserId = dbAuthUser.Id,
                    RoleId = roleId
                };
                _unitOfWork.Store(dbAuthUserRole, AuthUserDatabase.TempAuthUserRolesTable);
            }
        }
    }
}
