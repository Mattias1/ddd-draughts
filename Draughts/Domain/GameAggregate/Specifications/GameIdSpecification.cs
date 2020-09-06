using Draughts.Common.OoConcepts;
using Draughts.Domain.GameAggregate.Models;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.GameAggregate.Specifications {
    public class GameIdSpecification : Specification<Game> {
        private readonly GameId _id;

        public GameIdSpecification(GameId id) => _id = id;

        public override Expression<Func<Game, bool>> ToExpression() => g => g.Id == _id;
    }
}