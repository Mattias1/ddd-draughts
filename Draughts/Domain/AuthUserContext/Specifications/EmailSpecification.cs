using Draughts.Common.OoConcepts;
using Draughts.Domain.AuthUserContext.Models;
using SqlQueryBuilder.Builder;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.AuthUserContext.Specifications {
    public class EmailSpecification : Specification<AuthUser> {
        private readonly string? _email;

        public EmailSpecification(string? email) => _email = email;

        public override Expression<Func<AuthUser, bool>> ToExpression() => u => u.Email == _email;

        public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
            ApplyColumnWhere(builder, whereType, "email", q => q.Is(_email));
        }
    }
}
