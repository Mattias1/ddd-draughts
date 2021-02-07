using Draughts.Common.OoConcepts;
using Draughts.Domain.AuthUserAggregate.Models;
using SqlQueryBuilder.Builder;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.AuthUserAggregate.Specifications {
    public class AuthUserIdSpecification : Specification<AuthUser> {
        private readonly AuthUserId _id;

        public AuthUserIdSpecification(AuthUserId id) => _id = id;

        public override Expression<Func<AuthUser, bool>> ToExpression() => u => u.Id == _id;

        public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
            ApplyColumnWhere(builder, whereType, "id", q => q.Is(_id));
        }
    }
}
