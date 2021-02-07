using Draughts.Common.OoConcepts;
using Draughts.Domain.GameAggregate.Models;
using SqlQueryBuilder.Builder;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.GameAggregate.Specifications {
    public class FinishedGameSpecification : Specification<Game> {
        public override Expression<Func<Game, bool>> ToExpression() => g => g.IsFinished;

        public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
            ApplyColumnWhere(builder, whereType, "finished_at", q => q.IsNotNull());
        }
    }
}
