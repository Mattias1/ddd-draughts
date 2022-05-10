using Draughts.Domain.AuthContext.Models;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using static Draughts.Common.Events.DomainEvent;

namespace Draughts.Repositories.InMemory;

public sealed class InMemoryRoleRepository : InMemoryRepository<Role, RoleId>, IRoleRepository {
    public InMemoryRoleRepository(IRepositoryUnitOfWork unitOfWork) : base(unitOfWork) { }

    protected override IList<Role> GetBaseQuery() {
        var permissionRoles = AuthDatabase.Get.PermissionRolesTable.ToLookup(pr => pr.RoleId, pr => pr.Permission);
        return AuthDatabase.Get.RolesTable
            .Select(r => r.ToDomainModel(permissionRoles[r.Id].Select(p => new Permission(p)).ToArray()))
            .ToList();
    }

    public IReadOnlyList<Permission> PermissionsForRole(RoleId id) {
        var role = FindByIdOrNull(id);
        return role?.Permissions ?? new List<Permission>().AsReadOnly();
    }

    protected override void SaveInternal(Role entity) {
        var dbRole = DbRole.FromDomainModel(entity);
        UnitOfWork.Store(dbRole, tran => AuthDatabase.Temp(tran).RolesTable);

        foreach (var permission in entity.Permissions.Select(p => p.Value)) {
            var dbPermission = new DbPermissionRole {
                Permission = permission,
                RoleId = dbRole.Id
            };
            UnitOfWork.Store(dbPermission, tran => AuthDatabase.Temp(tran).PermissionRolesTable);
        }
    }

    public void Delete(RoleId roleId, DomainEventFactory eventFactory) {
        throw new NotImplementedException("Deleting data is not supported for the in memory database.");
    }
}
