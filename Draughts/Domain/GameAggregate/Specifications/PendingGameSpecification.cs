using Draughts.Common.OoConcepts;
using Draughts.Domain.GameAggregate.Models;
using SqlQueryBuilder.Builder;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.GameAggregate.Specifications {
    public class PendingGameSpecification : Specification<Game> {
        public override Expression<Func<Game, bool>> ToExpression() => g => !g.HasStarted;

        public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
            ApplyColumnWhere(builder, whereType, "started_at", q => q.IsNull());
        }
    }
}
