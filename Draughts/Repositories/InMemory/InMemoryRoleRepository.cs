using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.AuthUserAggregate.Specifications;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.InMemory {
    public class InMemoryRoleRepository : InMemoryRepository<Role>, IRoleRepository {
        private readonly IUnitOfWork _unitOfWork;

        public InMemoryRoleRepository(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        protected override IList<Role> GetBaseQuery() {
            var permissionRoles = AuthUserDatabase.PermissionRolesTable.ToLookup(pr => pr.RoleId, pr => pr.Permission);
            return AuthUserDatabase.RolesTable
                .Select(r => r.ToDomainModel(permissionRoles[r.Id].Select(p => new Permission(p)).ToArray()))
                .ToList();
        }

        public IReadOnlyList<Permission> PermissionsForRole(RoleId id) {
            var role = FindOrNull(new RoleIdSpecification(id));
            return role?.Permissions ?? new List<Permission>().AsReadOnly();
        }

        public override void Save(Role entity) {
            var dbRole = DbRole.FromDomainModel(entity);
            _unitOfWork.Store(dbRole, AuthUserDatabase.TempRolesTable);

            foreach (var permission in entity.Permissions.Select(p => p.Value)) {
                var dbPermission = new DbPermissionRole {
                    Permission = permission,
                    RoleId = dbRole.Id
                };
                _unitOfWork.Store(dbPermission, AuthUserDatabase.TempPermissionRolesTable);
            }
        }
    }
}
