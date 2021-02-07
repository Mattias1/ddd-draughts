using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.AuthUserAggregate.Specifications;
using Draughts.Repositories.Transaction;
using SqlQueryBuilder.Builder;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.Database {
    public class DbRoleRepository : DbRepository<Role, DbRole>, IRoleRepository {
        private readonly IUnitOfWork _unitOfWork;

        public DbRoleRepository(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public Role FindById(RoleId id) => Find(new RoleIdSpecification(id));
        public Role? FindByIdOrNull(RoleId id) => FindOrNull(new RoleIdSpecification(id));

        public IReadOnlyList<Permission> PermissionsForRole(RoleId id) {
            var role = FindByIdOrNull(id);
            return role?.Permissions ?? new List<Permission>().AsReadOnly();
        }

        protected override string TableName => "role";
        protected override IInitialQueryBuilder GetBaseQuery() => _unitOfWork.Query(TransactionDomain.AuthUser);
        private IQueryBuilder GetPermissionRoleQuery() => GetBaseQuery().SelectAllFrom("permission_role");

        protected override IReadOnlyList<Role> Parse(IReadOnlyList<DbRole> qs) {
            if (qs.Count == 0) {
                return new List<Role>().AsReadOnly();
            }

            var permissionRoles = GetPermissionRoleQuery().Where("role_id").In(qs.Select(q => q.Id)).List<DbPermissionRole>();
            var permissions = permissionRoles.ToLookup(pr => pr.RoleId, pr => new Permission(pr.Permission));
            return qs
                .Select(q => new Role(new RoleId(q.Id), q.Rolename, q.CreatedAt, permissions[q.Id].ToArray()))
                .ToList()
                .AsReadOnly();
        }

        protected override Role Parse(DbRole q) {
            var permissionRoles = GetPermissionRoleQuery().Where("role_id").Is(q.Id).List<DbPermissionRole>();
            var permissions = permissionRoles.Select(pr => new Permission(pr.Permission)).ToArray();
            return new Role(new RoleId(q.Id), q.Rolename, q.CreatedAt, permissions);
        }

        public override void Save(Role entity) {
            var obj = DbRole.FromDomainModel(entity);
            if (FindByIdOrNull(entity.Id) is null) {
                GetBaseQuery().InsertInto("role").InsertFrom(obj).Execute();
                InsertPermissions(entity.Id, entity.Permissions.Select(p => p.Value));
            }
            else {
                var oldPermissions = GetPermissionRoleQuery().Where("role_id").Is(entity.Id).List<DbPermissionRole>()
                    .Select(pr => pr.Permission).ToArray();
                var newPermissions = entity.Permissions.Select(p => p.Value).ToArray();
                var toDelete = oldPermissions.Except(newPermissions);
                var toAdd = newPermissions.Except(oldPermissions);

                GetBaseQuery().Update("role").SetWithoutIdFrom(obj).Where("id").Is(entity.Id).Execute();
                if (toDelete.Any()) {
                    GetBaseQuery().DeleteFrom("permission_role")
                        .Where("role_id").Is(entity.Id)
                        .And("permission").In(toDelete)
                        .Execute();
                }
                InsertPermissions(entity.Id, toAdd);
            }
        }

        private void InsertPermissions(long roleId, IEnumerable<string> permissions) {
            if (!permissions.Any()) {
                return;
            }
            var values = BuildInsertValues(roleId.ToString(), permissions);
            GetBaseQuery().InsertInto("permission_role")
                .Columns("role_id", "permission").Values(values)
                .Execute();
        }

        private IEnumerable<string> BuildInsertValues(string roleId, IEnumerable<string> permissions) {
            foreach (var permission in permissions) {
                yield return roleId;
                yield return permission;
            }
        }
    }
}
