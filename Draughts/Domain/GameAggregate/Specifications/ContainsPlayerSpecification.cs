using Draughts.Common.OoConcepts;
using Draughts.Domain.GameAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Draughts.Domain.GameAggregate.Specifications {
    public class ContainsPlayerSpecification : Specification<Game> {
        private readonly UserId _userId;

        public ContainsPlayerSpecification(UserId userId) => _userId = userId;

        public override Expression<Func<Game, bool>> ToExpression() => g => g.Players.Any(p => p.UserId == _userId);
    }
}
