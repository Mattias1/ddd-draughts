using NodaTime;
using SqlQueryBuilder.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlQueryBuilder.Builder;

public sealed partial class QueryBuilder : IComparisonQueryBuilder {
    public IQueryBuilder Eq(object? value) => Is(value);
    public IQueryBuilder Is(object? value) => value is null ? IsNull() : OperatorParameter("=", value);
    public IQueryBuilder IsNull() => OperatorParameter("is", null);

    public IQueryBuilder NotEq(object? value) => Isnt(value);
    public IQueryBuilder Isnt(object? value) => value is null ? IsNotNull() : OperatorParameter("!=", value);
    public IQueryBuilder IsntNull() => IsNotNull();
    public IQueryBuilder IsNotNull() => OperatorParameter("is not", null);

    public IQueryBuilder Gt(object? value) {
        return _options.SmartDate && value is LocalDate date
            ? OperatorParameter(">=", date.PlusDays(1))
            : OperatorParameter(">", value);
    }

    public IQueryBuilder GtEq(object? value) => OperatorParameter(">=", value);
    public IQueryBuilder Lt(object? value) => OperatorParameter("<", value);
    public IQueryBuilder LtEq(object? value) {
        return _options.SmartDate && value is LocalDate date
            ? OperatorParameter("<", date.PlusDays(1))
            : OperatorParameter("<=", value);
    }

    public IQueryBuilder Between(object? lower, object? upper) => DoubleOperatorParameter("between", lower, "and", upper);
    public IQueryBuilder NotBetween(object? lower, object? upper)
        => DoubleOperatorParameter("not between", lower, "and", upper);

    public IQueryBuilder Like(string value) => OperatorParameter("like", value);
    public IQueryBuilder NotLike(string value) => OperatorParameter("not like", value);

    public IQueryBuilder EqColumn(string column) => IsColumn(column);
    public IQueryBuilder IsColumn(string column) => OperatorColumn("=", column);

    public IQueryBuilder NotEqColumn(string column) => IsntColumn(column);
    public IQueryBuilder IsntColumn(string column) => OperatorColumn("!=", column);

    public IQueryBuilder GtColumn(string column) => OperatorColumn(">", column);
    public IQueryBuilder GtEqColumn(string column) => OperatorColumn(">=", column);
    public IQueryBuilder LtColumn(string column) => OperatorColumn("<", column);
    public IQueryBuilder LtEqColumn(string column) => OperatorColumn("<=", column);

    public IQueryBuilder In<T>(IEnumerable<T> enumerable) => OperatorArray("in", enumerable.Cast<object?>().ToArray());
    public IQueryBuilder In(params object?[] array) => OperatorArray("in", array);
    public IQueryBuilder NotIn<T>(IEnumerable<T> enumerable) => OperatorArray("not in", enumerable.Cast<object?>().ToArray());
    public IQueryBuilder NotIn(params object?[] array) => OperatorArray("not in", array);

    private IQueryBuilder OperatorParameter(string @operator, object? parameter) {
        if (_preparedLeaf is null) {
            throw new InvalidOperationException("You can only add a comparison after you've initiated a where, and or or.");
        }
        _preparedLeaf.Value.forest.Add(new WhereLeaf(_preparedLeaf.Value.type, _preparedLeaf.Value.column, @operator, parameter));
        return this;
    }

    private IQueryBuilder DoubleOperatorParameter(string operator1, object? p1, string operator2, object? p2) {
        if (_preparedLeaf is null) {
            throw new InvalidOperationException("You can only add a comparison after you've initiated a where, and or or.");
        }
        _preparedLeaf.Value.forest.Add(new WhereLeaf(_preparedLeaf.Value.type, _preparedLeaf.Value.column,
            new WhereLeaf.Comparison(operator1, p1), new WhereLeaf.Comparison(operator2, p2)));
        return this;
    }

    private IQueryBuilder OperatorColumn(string @operator, string column) {
        if (_preparedLeaf is null) {
            throw new InvalidOperationException("You can only add a comparison after you've initiated a where, and or or.");
        }
        _preparedLeaf.Value.forest.Add(new WhereLeaf(_preparedLeaf.Value.type, _preparedLeaf.Value.column,
            @operator, column, WhereLeaf.ValueType.Column));
        return this;
    }

    private IQueryBuilder OperatorArray(string @operator, object?[] array) {
        if (_preparedLeaf is null) {
            throw new InvalidOperationException("You can only add a comparison after you've initiated a where, and or or.");
        }
        if (array is null || array.Length == 0) {
            throw new InvalidOperationException("You can only add non-empty IN clauses.");
        }
        _preparedLeaf.Value.forest.Add(new WhereIn(_preparedLeaf.Value.type, _preparedLeaf.Value.column, @operator, array));
        return this;
    }

    public IQueryBuilder In(SubQueryFunc queryFunc) => SubqueryComparison("in", queryFunc);
    public IQueryBuilder NotIn(SubQueryFunc queryFunc) => SubqueryComparison("not in", queryFunc);

    public IQueryBuilder Eq(SubQueryFunc? queryFunc) => Is(queryFunc);
    public IQueryBuilder Is(SubQueryFunc? queryFunc) => queryFunc is null ? IsNull() : SubqueryComparison("=", queryFunc);
    public IQueryBuilder NotEq(SubQueryFunc? queryFunc) => Isnt(queryFunc);
    public IQueryBuilder Isnt(SubQueryFunc? queryFunc) => queryFunc is null ? IsNotNull() : SubqueryComparison("!=", queryFunc);

    public IQueryBuilder Gt(SubQueryFunc queryFunc) => SubqueryComparison(">", queryFunc);
    public IQueryBuilder GtEq(SubQueryFunc queryFunc) => SubqueryComparison(">=", queryFunc);
    public IQueryBuilder Lt(SubQueryFunc queryFunc) => SubqueryComparison("<", queryFunc);
    public IQueryBuilder LtEq(SubQueryFunc queryFunc) => SubqueryComparison("<=", queryFunc);

    private IQueryBuilder SubqueryComparison(string @operator, SubQueryFunc queryFunc) {
        if (_preparedLeaf is null) {
            throw new InvalidOperationException("You can only add a comparison after you've initiated a where, and or or.");
        }
        var queryBuilder = new QueryBuilder(_options);
        queryFunc(queryBuilder);
        _preparedLeaf.Value.forest.Add(new WhereSubQuery(_preparedLeaf.Value.type,
            _preparedLeaf.Value.column, @operator, queryBuilder));
        return this;
    }
}
