using Draughts.Common.OoConcepts;
using Draughts.Domain.UserAggregate.Models;
using SqlQueryBuilder.Builder;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.AuthUserAggregate.Specifications {
    public class UserIdSpecification : Specification<User> {
        private readonly UserId _id;

        public UserIdSpecification(UserId id) => _id = id;

        public override Expression<Func<User, bool>> ToExpression() => u => u.Id == _id;

        public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
            ApplyColumnWhere(builder, whereType, "id", q => q.Is(_id));
        }
    }
}
