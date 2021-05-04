using Draughts.Common.OoConcepts;
using Draughts.Domain.UserContext.Models;
using SqlQueryBuilder.Builder;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.UserContext.Specifications {
    public class RankSort : Sort<User, Rank> {
        public RankSort() : base(defaultDescending: true) { }
        public override Expression<Func<User, Rank>> ToExpression() => u => u.Rank;
        public override IQueryBuilder ApplyQueryBuilder(IQueryBuilder builder) => ApplyColumnSort(builder, "rank");
    }
}
