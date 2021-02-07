using Draughts.Common.OoConcepts;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using SqlQueryBuilder.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static Draughts.Repositories.Database.JoinEnum;

namespace Draughts.Domain.GameAggregate.Specifications {
    public class ContainsPlayerSpecification : Specification<Game> {
        private readonly UserId _userId;

        public ContainsPlayerSpecification(UserId userId) => _userId = userId;

        public override Expression<Func<Game, bool>> ToExpression() => g => g.Players.Any(p => p.UserId == _userId);

        public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
            ApplyColumnWhere(builder, whereType, "player.user_id", q => q.Is(_userId));
        }

        public override IEnumerable<PossibleJoins> RequiredJoins() {
            yield return PossibleJoins.Player;
        }
    }
}
