using Draughts.Common.OoConcepts;
using Draughts.Domain.GameContext.Models;
using SqlQueryBuilder.Builder;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.GameContext.Specifications;

public sealed class ActiveGameSpecification : Specification<Game> {
    public override Expression<Func<Game, bool>> ToExpression() => g => g.HasStarted && !g.IsFinished;

    public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
        ApplyFuncWhere(builder, whereType, q => q
            .Where("started_at").IsNotNull()
            .And("finished_at").IsNull()
        );
    }
}
