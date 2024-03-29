using Draughts.Common.OoConcepts;
using Draughts.Domain.AuthContext.Models;
using SqlQueryBuilder.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Draughts.Domain.AuthContext.Specifications;

public sealed class RoleIdsSpecification : Specification<Role> {
    private readonly RoleId[] _ids;

    public RoleIdsSpecification(IEnumerable<long> ids) : this(ids.Select(id => new RoleId(id)).ToArray()) { }
    public RoleIdsSpecification(params RoleId[] ids) => _ids = ids;

    public override Expression<Func<Role, bool>> ToExpression() => u => _ids.Contains(u.Id);

    public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
        ApplyColumnWhere(builder, whereType, "id", q => q.In(_ids.Select(i => i.Value)));
    }
}
