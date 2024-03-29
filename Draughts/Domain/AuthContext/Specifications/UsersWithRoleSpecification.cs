using Draughts.Common.OoConcepts;
using Draughts.Domain.AuthContext.Models;
using SqlQueryBuilder.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static Draughts.Repositories.Misc.JoinEnum;

namespace Draughts.Domain.AuthContext.Specifications;

public sealed class UsersWithRoleSpecification : Specification<AuthUser> {
    private readonly RoleId _roleId;

    public UsersWithRoleSpecification(RoleId roleId) => _roleId = roleId;

    public override Expression<Func<AuthUser, bool>> ToExpression() => u => u.RoleIds.Contains(_roleId);

    public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
        ApplyColumnWhere(builder, whereType, "authuser_roles.role_id", q => q.Is(_roleId.Value));
    }

    public override IEnumerable<PossibleJoins> RequiredJoins() {
        yield return PossibleJoins.AuthUserRole;
    }
}
