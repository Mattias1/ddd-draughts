using Draughts.Common.OoConcepts;
using Draughts.Domain.AuthUserAggregate.Models;
using SqlQueryBuilder.Builder;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.AuthUserAggregate.Specifications {
    public class RoleIdSpecification : Specification<Role> {
        private readonly RoleId _id;

        public RoleIdSpecification(RoleId id) => _id = id;

        public override Expression<Func<Role, bool>> ToExpression() => u => u.Id == _id;

        public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
            ApplyColumnWhere(builder, whereType, "id", q => q.Is(_id));
        }
    }
}
