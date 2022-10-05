using Draughts.Common.OoConcepts;
using Draughts.Domain.GameContext.Models;
using SqlQueryBuilder.Builder;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.GameContext.Specifications;

public sealed class GameIdSort : Sort<Game, GameId> {
    public GameIdSort() : base(defaultDescending: true) { }
    public override Expression<Func<Game, GameId>> ToExpression() => g => g.Id;
    public override IQueryBuilder ApplyQueryBuilder(IQueryBuilder builder) => ApplyColumnSort(builder, "games.id");
}
