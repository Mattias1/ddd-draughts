using Draughts.Common.OoConcepts;
using Draughts.Domain.AuthUserAggregate.Models;
using SqlQueryBuilder.Builder;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.AuthUserAggregate.Specifications {
    public class RolenameSpecification : Specification<Role> {
        private readonly string _rolename;

        public RolenameSpecification(string rolename) => _rolename = rolename;

        public override Expression<Func<Role, bool>> ToExpression() {
            return u => u.Rolename.Equals(_rolename, StringComparison.OrdinalIgnoreCase);
        }

        public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
            ApplyColumnWhere(builder, whereType, "rolename", q => q.Is(_rolename));
        }
    }
}
