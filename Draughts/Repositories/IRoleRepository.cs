using Draughts.Domain.AuthContext.Models;
using System.Collections.Generic;
using static Draughts.Common.Events.DomainEvent;

namespace Draughts.Repositories;

public interface IRoleRepository : IRepository<Role, RoleId> {
    IReadOnlyList<Permission> PermissionsForRole(RoleId roleId);
    void Delete(RoleId roleId, DomainEventFactory eventFactory);
}
