using Draughts.Common.OoConcepts;
using Draughts.Domain.GameAggregate.Models;
using SqlQueryBuilder.Builder;
using System;
using System.Linq.Expressions;

namespace Draughts.Domain.GameAggregate.Specifications {
    public class PlayerIdSpecification : Specification<Player> {
        private readonly PlayerId _id;

        public PlayerIdSpecification(PlayerId id) => _id = id;

        public override Expression<Func<Player, bool>> ToExpression() => g => g.Id == _id;

        public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
            ApplyColumnWhere(builder, whereType, "id", q => q.Is(_id));
        }
    }
}
