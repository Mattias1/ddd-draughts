using SqlQueryBuilder.Exceptions;
using SqlQueryBuilder.Model;
using SqlQueryBuilder.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlQueryBuilder.Builder;

public sealed partial class QueryBuilder : IQueryBuilderBase {
    public delegate IQueryBuilder SubQueryFunc(IInitialQueryBuilder builder);
    public delegate IQueryBuilder SubWhereFunc(IQueryBuilder builder);

    private readonly QueryBuilderOptions _options;
    private readonly Query _query;
    private bool _explicitlyWithoutWhere;

    public static IInitialQueryBuilder Init(ISqlFlavor sqlFlavor) => Init(new QueryBuilderOptions(sqlFlavor));
    public static IInitialQueryBuilder Init(ISqlFlavor sqlFlavor, IColumnFormat columnFormat)
        => Init(new QueryBuilderOptions(sqlFlavor, columnFormat));
    public static IInitialQueryBuilder Init(QueryBuilderOptions options) => new QueryBuilder(options);

    internal QueryBuilder(QueryBuilderOptions options) : this(options, new Query(options)) { }

    private QueryBuilder(QueryBuilderOptions options, Query query) {
        _options = options;
        _query = query;
        _explicitlyWithoutWhere = false;
    }

    public ICompleteQueryBuilder Cast() => this;

    public async Task<IReadOnlyList<T>> ListAsync<T>() {
        string parameterizedSql = ToParameterizedSql();
        try {
            return await _options.SqlFlavor.ExecuteWithResultsAsync<T>(parameterizedSql, _query.Parameters);
        } catch (Exception e) {
            throw new SqlExecutionException(e, _options.AddParameterizedSqlToException ? parameterizedSql : null);
        }
    }

    public IReadOnlyList<T> List<T>() {
        string parameterizedSql = ToParameterizedSql();
        try {
            return _options.SqlFlavor.ExecuteWithResults<T>(parameterizedSql, _query.Parameters);
        } catch (Exception e) {
            throw new SqlExecutionException(e, _options.AddParameterizedSqlToException ? parameterizedSql : null);
        }
    }

    public bool Execute() {
        string parameterizedSql = ToParameterizedSql();
        try {
            return _options.SqlFlavor.Execute(parameterizedSql, _query.Parameters);
        } catch (Exception e) {
            throw new SqlExecutionException(e, _options.AddParameterizedSqlToException ? parameterizedSql : null);
        }
    }

    public async Task<bool> ExecuteAsync() {
        string parameterizedSql = ToParameterizedSql();
        try {
            return await _options.SqlFlavor.ExecuteAsync(parameterizedSql, _query.Parameters);
        } catch (Exception e) {
            throw new SqlExecutionException(e, _options.AddParameterizedSqlToException ? parameterizedSql : null);
        }
    }

    public override string ToString() => ToParameterizedSql();

    public string ToUnsafeSql() {
        string query = ToParameterizedSql();
        foreach (var (key, value) in _query.Parameters) {
            if (value is string stringValue) {
                query = query.Replace(key, $"'{stringValue}'");
            } else {
                query = query.Replace(key, value?.ToString() ?? "null");
            }
        }
        return query;
    }

    internal (string sql, Dictionary<string, object?> parameters) ToParameterizedSqlWithParams() {
        string sql = ToParameterizedSql();
        return (sql, _query.Parameters);
    }

    public string ToParameterizedSql() {
        if (_options.GuardForForgottenWhere && !_explicitlyWithoutWhere
                && _query.WhereForest.Count == 0
                && (_query.UpdateTable is not null || _query.DeleteTable is not null)) {
            string aQueryType = _query.UpdateTable is not null ? "an update" : "a delete";
            throw new SqlQueryBuilderException($"You are trying to execute {aQueryType} query without a where. "
                + "If this is intentional, please specify by calling '.WithoutWhere()'.");
        }
        string query = _query.ToParameterizedSql();
        int semiColonIndex = query.IndexOf(';');
        if (_options.OverprotectiveSqlInjection) {
            if (semiColonIndex >= 0 && semiColonIndex != query.Length - 1) {
                throw new PotentialSqlInjectionException(";");
            }
            if (query.Contains("--")) {
                throw new PotentialSqlInjectionException("--");
            }
        }
        return query;
    }

    public ICompleteQueryBuilder Clone() => new QueryBuilder(_options.Clone(), _query.Clone());
    public ICompleteQueryBuilder CloneWithoutSelect() {
        var query = _query.Clone();
        query.SelectColumns.Clear();
        return new QueryBuilder(_options.Clone(), query);
    }
}

public sealed partial class QueryBuilder : ICompleteQueryBuilder { }
