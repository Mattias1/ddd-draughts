using Draughts.Common.OoConcepts;
using Draughts.Domain.GameAggregate.Models;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.GameAggregate.Specifications {
    public class FinishedGameSpecification : Specification<Game> {
        public override Expression<Func<Game, bool>> ToExpression() => g => g.IsFinished;
    }
}
