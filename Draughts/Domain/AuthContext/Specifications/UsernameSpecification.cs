using Draughts.Common.OoConcepts;
using Draughts.Domain.AuthContext.Models;
using SqlQueryBuilder.Builder;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.AuthContext.Specifications {
    public class UsernameSpecification : Specification<AuthUser> {
        private readonly string? _username;

        public UsernameSpecification(Username? username) : this(username?.Value) { }
        public UsernameSpecification(string? username) => _username = username;

        public override Expression<Func<AuthUser, bool>> ToExpression() => u => u.Username == _username;

        public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
            ApplyColumnWhere(builder, whereType, "username", q => q.Is(_username));
        }
    }
}
