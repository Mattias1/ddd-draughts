using Draughts.Common;
using Draughts.Domain.AuthUserAggregate.Models;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.AuthUserAggregate.Specifications {
    public class AuthUserIdSpecification : Specification<AuthUser> {
        private readonly AuthUserId _id;

        public AuthUserIdSpecification(AuthUserId id) => _id = id;

        public override Expression<Func<AuthUser, bool>> ToExpression() => u => u.Id == _id;
    }
}
