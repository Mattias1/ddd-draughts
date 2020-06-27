using Draughts.Common;
using Draughts.Domain.AuthUserAggregate.Models;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.AuthUserAggregate.Specifications {
    public class RolenameSpecification : Specification<Role> {
        private readonly string _rolename;

        public RolenameSpecification(string rolename) => _rolename = rolename;

        public override Expression<Func<Role, bool>> ToExpression() {
            return u => u.Rolename.Equals(_rolename, StringComparison.OrdinalIgnoreCase);
        }
    }
}
