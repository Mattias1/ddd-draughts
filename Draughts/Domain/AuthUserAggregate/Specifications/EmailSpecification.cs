using Draughts.Common;
using Draughts.Domain.AuthUserAggregate.Models;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.AuthUserAggregate.Specifications {
    public class EmailSpecification : Specification<AuthUser> {
        private readonly string? _email;

        public EmailSpecification(string? email) => _email = email;

        public override Expression<Func<AuthUser, bool>> ToExpression() => u => u.Email == _email;
    }
}
