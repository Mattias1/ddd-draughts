using System.Collections.Generic;

namespace SqlQueryBuilder.Builder {
    public interface IComparisonQueryBuilder {
        IQueryBuilder Eq(object? value);
        IQueryBuilder Is(object? value);
        IQueryBuilder IsNull();

        IQueryBuilder Neq(object? value);
        IQueryBuilder NotEq(object? value);
        IQueryBuilder Isnt(object? value);
        IQueryBuilder IsntNull();
        IQueryBuilder IsNotNull();

        IQueryBuilder Gt(object? value);
        IQueryBuilder GtEq(object? value);

        IQueryBuilder Lt(object? value);
        IQueryBuilder LtEq(object? value);

        IQueryBuilder Between(object? lower, object? upper);
        IQueryBuilder NotBetween(object? lower, object? upper);

        IQueryBuilder Like(string value);
        IQueryBuilder NotLike(string value);

        IQueryBuilder EqColumn(string column);
        IQueryBuilder IsColumn(string column);

        IQueryBuilder NeqColumn(string column);
        IQueryBuilder NotEqColumn(string column);
        IQueryBuilder IsntColumn(string column);

        IQueryBuilder GtColumn(string column);
        IQueryBuilder GtEqColumn(string column);
        IQueryBuilder LtColumn(string column);
        IQueryBuilder LtEqColumn(string column);
        IQueryBuilder In<T>(IEnumerable<T> enumerable);
        IQueryBuilder In(params object?[] array);
        IQueryBuilder NotIn<T>(IEnumerable<T> enumerable);
        IQueryBuilder NotIn(params object?[] array);
    }
}