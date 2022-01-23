using System.Collections.Generic;
using static SqlQueryBuilder.Builder.QueryBuilder;

namespace SqlQueryBuilder.Builder;

public interface IComparisonQueryBuilder {
    IQueryBuilder Eq(object? value);
    IQueryBuilder Is(object? value);
    IQueryBuilder IsNull();

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

    IQueryBuilder In(SubQueryFunc queryFunc);
    IQueryBuilder NotIn(SubQueryFunc queryFunc);

    IQueryBuilder Eq(SubQueryFunc? queryFunc);
    IQueryBuilder Is(SubQueryFunc? queryFunc);
    IQueryBuilder NotEq(SubQueryFunc? queryFunc);
    IQueryBuilder Isnt(SubQueryFunc? queryFunc);

    IQueryBuilder Gt(SubQueryFunc queryFunc);
    IQueryBuilder GtEq(SubQueryFunc queryFunc);
    IQueryBuilder Lt(SubQueryFunc queryFunc);
    IQueryBuilder LtEq(SubQueryFunc queryFunc);
}
