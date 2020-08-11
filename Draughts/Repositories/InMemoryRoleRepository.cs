using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.AuthUserAggregate.Specifications;
using Draughts.Repositories.Database;
using Draughts.Repositories.Databases;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories {
    public class InMemoryRoleRepository : InMemoryRepository<Role>, IRoleRepository {
        private readonly IUnitOfWork _unitOfWork;

        public InMemoryRoleRepository(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        protected override IList<Role> GetBaseQuery() {
            return AuthUserDatabase.RolesTable.Select(r => new Role(
                new RoleId(r.Id),
                r.Rolename,
                r.Permissions.Select(p => new Permission(p)).ToArray()
            )).ToList();
        }

        public IReadOnlyList<Permission> PermissionsForRole(RoleId id) {
            var role = FindOrNull(new RoleIdSpecification(id));
            return role?.Permissions ?? new List<Permission>().AsReadOnly();
        }

        public override void Save(Role entity) {
            var role = new InMemoryRole {
                Id = entity.Id,
                Rolename = entity.Rolename,
                Permissions = entity.Permissions.Select(p => p.Value).ToArray()
            };

            _unitOfWork.Store(role, AuthUserDatabase.TempRolesTable);
        }
    }
}
