using Draughts.Common;
using Draughts.Domain.AuthUserAggregate.Models;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.AuthUserAggregate.Specifications {
    public class RoleIdSpecification : Specification<Role> {
        private readonly RoleId _id;

        public RoleIdSpecification(RoleId id) => _id = id;

        public override Expression<Func<Role, bool>> ToExpression() => u => u.Id == _id;
    }
}
