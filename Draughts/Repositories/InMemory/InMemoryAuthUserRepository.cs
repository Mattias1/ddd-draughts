using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.AuthContext.Specifications;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.InMemory;

public sealed class InMemoryAuthUserRepository : InMemoryRepository<AuthUser, UserId>, IAuthUserRepository {
    private readonly IRoleRepository _roleRepository;

    public InMemoryAuthUserRepository(IRoleRepository roleRepository, IRepositoryUnitOfWork unitOfWork)
            : base(unitOfWork) {
        _roleRepository = roleRepository;
    }

    public AuthUser FindByName(string username) => Find(new UsernameSpecification(username));

    protected override IList<AuthUser> GetBaseQuery() {
        var authUserRoles = AuthDatabase.Get.AuthUserRolesTable.ToLookup(ar => ar.UserId, ar => new RoleId(ar.RoleId));
        return AuthDatabase.Get.AuthUsersTable
            .Select(u => u.ToDomainModel(authUserRoles[u.Id].ToList().AsReadOnly()))
            .ToList();
    }

    protected override void SaveInternal(AuthUser entity) {
        var dbAuthUser = DbAuthUser.FromDomainModel(entity);
        UnitOfWork.Store(dbAuthUser, tran => AuthDatabase.Temp(tran).AuthUsersTable);

        foreach (var roleId in entity.RoleIds.Select(r => r.Value)) {
            var dbAuthUserRole = new DbAuthUserRole {
                UserId = dbAuthUser.Id,
                RoleId = roleId
            };
            UnitOfWork.Store(dbAuthUserRole, tran => AuthDatabase.Temp(tran).AuthUserRolesTable);
        }
    }
}
