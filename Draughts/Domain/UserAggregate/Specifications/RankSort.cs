using Draughts.Common.OoConcepts;
using Draughts.Domain.UserAggregate.Models;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.UserAggregate.Specifications {
    public class RankSort : Sort<User, Rank> {
        public RankSort() : base(defaultDescending: true) { }
        public override Expression<Func<User, Rank>> ToExpression() => u => u.Rank;
    }
}
