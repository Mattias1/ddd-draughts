using SqlQueryBuilder.Builder;
using System;
using System.Linq.Expressions;

namespace Draughts.Common.OoConcepts;

public sealed class EntityIdSpecification<T, TId> : Specification<T> where T : Entity<T, TId> where TId : IdValueObject<TId> {
    private readonly TId _id;
    private readonly string _fieldName;

    public EntityIdSpecification(TId id, string fieldName = "id") {
        _id = id;
        _fieldName = fieldName;
    }

    public override Expression<Func<T, bool>> ToExpression() => u => _id.Equals(u.Id);

    public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
        ApplyColumnWhere(builder, whereType, _fieldName, q => q.Is(_id.Value));
    }
}
