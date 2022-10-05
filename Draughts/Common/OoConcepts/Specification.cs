using SqlQueryBuilder.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static Draughts.Repositories.Misc.JoinEnum;

namespace Draughts.Common.OoConcepts;

public abstract class Specification<T> {
    public enum QueryWhereType { And, Or, AndNot, OrNot };

    public abstract Expression<Func<T, bool>> ToExpression();

    public bool IsSatisfiedBy(T entity) {
        Func<T, bool> predicate = ToExpression().Compile();
        return predicate(entity);
    }

    public IQueryBuilder ApplyQueryBuilder(IQueryBuilder builder) {
        ApplyQueryBuilder(builder, QueryWhereType.And);
        return builder;
    }
    public abstract void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType);

    protected void ApplyColumnWhere(IQueryBuilder builder, QueryWhereType whereType, string column,
            Func<IComparisonQueryBuilder, IQueryBuilder> func) {
        if (whereType == QueryWhereType.And) {
            func(builder.And(column));
        }
        else if (whereType == QueryWhereType.Or) {
            func(builder.Or(column));
        }
        else if (whereType == QueryWhereType.AndNot) {
            builder.AndNot(builder => func(builder.Where(column)));
        }
        else if (whereType == QueryWhereType.OrNot) {
            builder.OrNot(builder => func(builder.Where(column)));
        }
        else {
            throw new InvalidOperationException("Unknown query where type");
        }
    }

    protected void ApplyFuncWhere(IQueryBuilder builder, QueryWhereType whereType, QueryBuilder.SubWhereFunc func) {
        if (whereType == QueryWhereType.And) {
            builder.And(func);
        }
        else if (whereType == QueryWhereType.Or) {
            builder.Or(func);
        }
        else if (whereType == QueryWhereType.AndNot) {
            builder.AndNot(func);
        }
        else if (whereType == QueryWhereType.OrNot) {
            builder.OrNot(func);
        }
        else {
            throw new InvalidOperationException("Unknown query where type");
        }
    }

    public virtual IEnumerable<PossibleJoins> RequiredJoins() => new PossibleJoins[0];

    public Specification<T> And(Specification<T> specification) => new AndSpecification<T>(this, specification);
    public Specification<T> Or(Specification<T> specification) => new OrSpecification<T>(this, specification);
    public Specification<T> Not() => new NotSpecification<T>(this);
}

public sealed class AndSpecification<T> : Specification<T> {
    private readonly Specification<T> _left, _right;

    public AndSpecification(Specification<T> left, Specification<T> right) => (_left, _right) = (left, right);

    public override Expression<Func<T, bool>> ToExpression() {
        Expression<Func<T, bool>> leftExpression = _left.ToExpression();
        Expression<Func<T, bool>> rightExpression = _right.ToExpression();
        InvocationExpression invokedRightExpression = Expression.Invoke(rightExpression, leftExpression.Parameters);
        BinaryExpression andExpression = Expression.AndAlso(leftExpression.Body, invokedRightExpression);

        return Expression.Lambda<Func<T, bool>>(andExpression, leftExpression.Parameters.Single());
    }

    public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
        _left.ApplyQueryBuilder(builder, whereType);
        _right.ApplyQueryBuilder(builder, QueryWhereType.And);
    }

    public override IEnumerable<PossibleJoins> RequiredJoins() => _left.RequiredJoins().Concat(_right.RequiredJoins());
}

public sealed class OrSpecification<T> : Specification<T> {
    private readonly Specification<T> _left, _right;

    public OrSpecification(Specification<T> left, Specification<T> right) => (_left, _right) = (left, right);

    public override Expression<Func<T, bool>> ToExpression() {
        Expression<Func<T, bool>> leftExpression = _left.ToExpression();
        Expression<Func<T, bool>> rightExpression = _right.ToExpression();
        InvocationExpression invokedRightExpression = Expression.Invoke(rightExpression, leftExpression.Parameters);
        BinaryExpression orExpression = Expression.OrElse(leftExpression.Body, invokedRightExpression);

        return Expression.Lambda<Func<T, bool>>(orExpression, leftExpression.Parameters.Single());
    }

    public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
        _left.ApplyQueryBuilder(builder, whereType);
        _right.ApplyQueryBuilder(builder, QueryWhereType.Or);
    }

    public override IEnumerable<PossibleJoins> RequiredJoins() => _left.RequiredJoins().Concat(_right.RequiredJoins());
}

public sealed class NotSpecification<T> : Specification<T> {
    private readonly Specification<T> _specification;

    public NotSpecification(Specification<T> specification) => _specification = specification;

    public override Expression<Func<T, bool>> ToExpression() {
        Expression<Func<T, bool>> expression = _specification.ToExpression();
        UnaryExpression notExpression = Expression.Not(expression.Body);

        return Expression.Lambda<Func<T, bool>>(notExpression, expression.Parameters.Single());
    }

    public override void ApplyQueryBuilder(IQueryBuilder builder, QueryWhereType whereType) {
        var newWhereType = whereType switch {
            QueryWhereType.And => QueryWhereType.AndNot,
            QueryWhereType.Or => QueryWhereType.OrNot,
            QueryWhereType.AndNot => QueryWhereType.And,
            QueryWhereType.OrNot => QueryWhereType.Or,
            _ => throw new InvalidOperationException("Unknown query where type")
        };
        _specification.ApplyQueryBuilder(builder, newWhereType);
    }

    public override IEnumerable<PossibleJoins> RequiredJoins() => _specification.RequiredJoins();
}

public static class QueryBuilderSpecificationExtension {
    public static IQueryBuilder Specifically<T>(this IQueryBuilder builder, Specification<T> spec) {
        return spec.ApplyQueryBuilder(builder);
    }
}
