using Draughts.Domain.AuthContext.Models;
using Draughts.Repositories.Misc;
using Draughts.Repositories.Transaction;
using SqlQueryBuilder.Builder;
using System.Collections.Generic;
using System.Linq;
using static Draughts.Common.Events.DomainEvent;

namespace Draughts.Repositories;

public sealed class RoleRepository : BaseRepository<Role, RoleId, DbRole> {
    public RoleRepository(IRepositoryUnitOfWork unitOfWork) : base(unitOfWork) { }

    public IReadOnlyList<Permission> PermissionsForRole(RoleId id) {
        var role = FindByIdOrNull(id);
        return role?.Permissions ?? new List<Permission>().AsReadOnly();
    }

    protected override string TableName => "role";
    private const string PermissionRoleTableName = "permission_role";
    protected override IInitialQueryBuilder GetBaseQuery() => UnitOfWork.Query(TransactionDomain.Auth);
    private IQueryBuilder GetPermissionRoleQuery() => GetBaseQuery().SelectAllFrom(PermissionRoleTableName);

    protected override IReadOnlyList<Role> Parse(IReadOnlyList<DbRole> qs) {
        if (qs.Count == 0) {
            return new List<Role>().AsReadOnly();
        }

        var permissionRoles = GetPermissionRoleQuery().Where("role_id").In(qs.Select(q => q.Id)).List<DbPermissionRole>();
        var permissions = permissionRoles.ToLookup(pr => pr.RoleId, pr => new Permission(pr.Permission));
        return qs
            .Select(q => q.ToDomainModel(permissions[q.Id].ToArray()))
            .ToList()
            .AsReadOnly();
    }

    protected override Role Parse(DbRole q) {
        var permissionRoles = GetPermissionRoleQuery().Where("role_id").Is(q.Id).List<DbPermissionRole>();
        var permissions = permissionRoles.Select(pr => new Permission(pr.Permission)).ToArray();
        return q.ToDomainModel(permissions);
    }

    protected override void SaveInternal(Role entity) {
        var obj = DbRole.FromDomainModel(entity);
        if (FindByIdOrNull(entity.Id) is null) {
            GetBaseQuery().InsertInto(TableName).InsertFrom(obj).Execute();
            InsertPermissions(entity.Id.Value, entity.Permissions.Select(p => p.Value));
        }
        else {
            var oldPermissions = GetPermissionRoleQuery().Where("role_id").Is(entity.Id).List<DbPermissionRole>()
                .Select(pr => pr.Permission).ToArray();
            var newPermissions = entity.Permissions.Select(p => p.Value).ToArray();
            var toDelete = oldPermissions.Except(newPermissions);
            var toAdd = newPermissions.Except(oldPermissions);

            GetBaseQuery().Update(TableName).SetWithoutIdFrom(obj).Where("id").Is(entity.Id).Execute();
            if (toDelete.Any()) {
                GetBaseQuery().DeleteFrom(PermissionRoleTableName)
                    .Where("role_id").Is(entity.Id)
                    .And("permission").In(toDelete)
                    .Execute();
            }
            InsertPermissions(entity.Id.Value, toAdd);
        }
    }

    private void InsertPermissions(long roleId, IEnumerable<string> permissions) {
        if (!permissions.Any()) {
            return;
        }
        var values = BuildInsertValues(roleId.ToString(), permissions);
        GetBaseQuery().InsertInto(PermissionRoleTableName)
            .Columns("role_id", "permission").Values(values)
            .Execute();
    }

    private IEnumerable<string> BuildInsertValues(string roleId, IEnumerable<string> permissions) {
        foreach (var permission in permissions) {
            yield return roleId;
            yield return permission;
        }
    }

    public void Delete(RoleId roleId, DomainEventFactory eventFactory) {
        UnitOfWork.Raise(eventFactory);

        GetBaseQuery().DeleteFrom(PermissionRoleTableName).Where("role_id").Is(roleId).Execute();
        GetBaseQuery().DeleteFrom(TableName).Where("id").Is(roleId).Execute();
    }
}
