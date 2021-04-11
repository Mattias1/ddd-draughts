using Draughts.Common.OoConcepts;
using Draughts.Domain.GameAggregate.Models;
using SqlQueryBuilder.Builder;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.GameAggregate.Specifications {
    public class GameIdSort : Sort<Game, GameId> {
        public GameIdSort() : base(defaultDescending: true) { }
        public override Expression<Func<Game, GameId>> ToExpression() => g => g.Id;
        public override IQueryBuilder ApplyQueryBuilder(IQueryBuilder builder) => ApplyColumnSort(builder, "game.id");
    }
}
