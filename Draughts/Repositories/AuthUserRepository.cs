using Draughts.Common.OoConcepts;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.AuthContext.Specifications;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories.Misc;
using Draughts.Repositories.Transaction;
using SqlQueryBuilder.Builder;
using System.Collections.Generic;
using System.Linq;
using static Draughts.Repositories.Misc.JoinEnum;

namespace Draughts.Repositories;

public sealed class AuthUserRepository : BaseRepository<AuthUser, UserId, DbAuthUser> {
    private readonly RoleRepository _roleRepository;

    public AuthUserRepository(RoleRepository roleRepository, IRepositoryUnitOfWork unitOfWork)
            : base(unitOfWork) {
        _roleRepository = roleRepository;
    }

    public AuthUser FindByName(string username) => Find(new UsernameSpecification(username));

    protected override string TableName => "authuser";
    protected override IInitialQueryBuilder GetBaseQuery() => UnitOfWork.Query(TransactionDomain.Auth);
    private IQueryBuilder GetAuthuserRoleQuery() => GetBaseQuery().SelectAllFrom("authuser_role");

    protected override IQueryBuilder ApplySpec(Specification<AuthUser> spec, IQueryBuilder builder) {
        var joins = spec.RequiredJoins().ToArray();
        if (joins.Contains(PossibleJoins.AuthUserRole)) {
            builder.Join("authuser_role", "authuser.id", "authuser_role.user_id");
        }
        return base.ApplySpec(spec, builder);
    }

    protected override IReadOnlyList<AuthUser> Parse(IReadOnlyList<DbAuthUser> qs) {
        if (qs.Count == 0) {
            return new List<AuthUser>().AsReadOnly();
        }

        var userRoles = GetAuthuserRoleQuery().Where("user_id").In(qs.Select(q => q.Id)).List<DbAuthUserRole>();
        var roleIds = userRoles.ToLookup(ur => ur.UserId, ur => new RoleId(ur.RoleId));
        return qs
            .Select(q => q.ToDomainModel(roleIds[q.Id].ToList()))
            .ToList()
            .AsReadOnly();
    }

    protected override AuthUser Parse(DbAuthUser authUser) {
        var roleIds = QueryRoleIdsForUser(authUser.Id).Select(id => new RoleId(id));
        return authUser.ToDomainModel(roleIds);
    }

    protected override void SaveInternal(AuthUser entity) {
        var obj = DbAuthUser.FromDomainModel(entity);
        if (FindByIdOrNull(entity.Id) is null) {
            GetBaseQuery().InsertInto(TableName).InsertFrom(obj).Execute();
            InsertRoles(entity.Id, entity.RoleIds.Select(r => r.Value));
        }
        else {
            var oldRoleIds = QueryRoleIdsForUser(entity.Id.Value).ToArray();
            var newRoleIds = entity.RoleIds.Select(p => p.Value).ToArray();
            var toDelete = oldRoleIds.Except(newRoleIds).ToArray();
            var toAdd = newRoleIds.Except(oldRoleIds);

            GetBaseQuery().Update(TableName).SetWithoutIdFrom(obj).Where("id").Is(entity.Id).Execute();
            if (toDelete.Any()) {
                GetBaseQuery().DeleteFrom("authuser_role")
                    .Where("user_id").Is(entity.Id)
                    .And("role_id").In(toDelete)
                    .Execute();
            }
            InsertRoles(entity.Id, toAdd);
        }
    }

    private void InsertRoles(UserId userId, IEnumerable<long> roleIds) {
        if (!roleIds.Any()) {
            return;
        }
        var values = BuildInsertValues(userId.Value, roleIds);
        GetBaseQuery().InsertInto("authuser_role")
            .Columns("user_id", "role_id").Values(values)
            .Execute();
    }

    private IEnumerable<long> BuildInsertValues(long userId, IEnumerable<long> roleIds) {
        foreach (var roleId in roleIds) {
            yield return userId;
            yield return roleId;
        }
    }

    private IReadOnlyList<long> QueryRoleIdsForUser(long authUserId) {
        return GetBaseQuery()
            .Select("role_id")
            .From("authuser_role")
            .Where("user_id").Is(authUserId)
            .ListLongs();
    }
}
