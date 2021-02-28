using SqlQueryBuilder.Builder;
using System;
using System.Linq.Expressions;

namespace Draughts.Common.OoConcepts {
    public class EntityIdSpecification<T, TId> : Specification<T> where T : Entity<T, TId> where TId : IdValueObject<TId> {
        private readonly TId _id;

        public EntityIdSpecification(TId id) => _id = id;

        public override Expression<Func<T, bool>> ToExpression() => u => _id.Equals(u.Id);

        public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
            ApplyColumnWhere(builder, whereType, "id", q => q.Is(_id));
        }
    }
}
