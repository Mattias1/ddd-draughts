using Draughts.Common.OoConcepts;
using Draughts.Domain.AuthUserAggregate.Models;
using SqlQueryBuilder.Builder;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.AuthUserAggregate.Specifications {
    public class UsernameSpecification : Specification<AuthUser> {
        private readonly string? _username;

        public UsernameSpecification(string? username) => _username = username;

        public override Expression<Func<AuthUser, bool>> ToExpression() => u => u.Username == _username;

        public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
            ApplyColumnWhere(builder, whereType, "username", q => q.Is(_username));
        }
    }
}
