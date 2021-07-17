using SqlQueryBuilder.Common;
using SqlQueryBuilder.Exceptions;
using SqlQueryBuilder.Model;
using SqlQueryBuilder.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlQueryBuilder.Builder {
    public partial class QueryBuilder : IQueryBuilderBase {
        private readonly QueryBuilderOptions _options;
        private readonly Query _query;

        public static IInitialQueryBuilder Init(ISqlFlavor sqlFlavor) => Init(new QueryBuilderOptions(sqlFlavor));
        public static IInitialQueryBuilder Init(ISqlFlavor sqlFlavor, IColumnFormat columnFormat)
            => Init(new QueryBuilderOptions(sqlFlavor, columnFormat));
        public static IInitialQueryBuilder Init(QueryBuilderOptions options) => new QueryBuilder(options);

        internal QueryBuilder(QueryBuilderOptions options) : this(options, new Query(options)) { }

        private QueryBuilder(QueryBuilderOptions options, Query query) {
            _options = options;
            _query = query;
        }

        public ICompleteQueryBuilder Cast() => this;

        public int FirstInt() => FirstValue<int>();
        public int FirstInt(string column) => FirstValue<int>(column);
        public int SingleInt() => SingleValue<int>();
        public int SingleInt(string column) => SingleValue<int>(column);
        public IReadOnlyList<int> ListInts() => ListValues<int>();
        public IReadOnlyList<int> ListInts(string column) => ListValues<int>(column);
        public async Task<int> FirstIntAsync() => await FirstValueAsync<int>();
        public async Task<int> FirstIntAsync(string column) => await FirstValueAsync<int>(column);
        public async Task<int> SingleIntAsync() => await SingleValueAsync<int>();
        public async Task<int> SingleIntAsync(string column) => await SingleValueAsync<int>(column);
        public async Task<IReadOnlyList<int>> ListIntsAsync() => await ListValuesAsync<int>();
        public async Task<IReadOnlyList<int>> ListIntsAsync(string column) => await ListValuesAsync<int>(column);

        public long FirstLong() => FirstValue<long>();
        public long FirstLong(string column) => FirstValue<long>(column);
        public long SingleLong() => SingleValue<long>();
        public long SingleLong(string column) => SingleValue<long>(column);
        public IReadOnlyList<long> ListLongs() => ListValues<long>();
        public IReadOnlyList<long> ListLongs(string column) => ListValues<long>(column);
        public async Task<long> FirstLongAsync() => await FirstValueAsync<long>();
        public async Task<long> FirstLongAsync(string column) => await FirstValueAsync<long>(column);
        public async Task<long> SingleLongAsync() => await SingleValueAsync<long>();
        public async Task<long> SingleLongAsync(string column) => await SingleValueAsync<long>(column);
        public async Task<IReadOnlyList<long>> ListLongsAsync() => await ListValuesAsync<long>();
        public async Task<IReadOnlyList<long>> ListLongsAsync(string column) => await ListValuesAsync<long>(column);

        public string FirstString() => FirstValue<string>();
        public string FirstString(string column) => FirstValue<string>(column);
        public string? FirstOrDefaultString() => RowClass<string>(FirstOrDefaultResult());
        public string? FirstOrDefaultString(string column) => RowClass<string>(FirstOrDefaultResult(), column);
        public string SingleString() => SingleValue<string>();
        public string SingleString(string column) => SingleValue<string>(column);
        public string? SingleOrDefaultString() => RowClass<string>(SingleOrDefaultResult());
        public string? SingleOrDefaultString(string column) => RowClass<string>(SingleOrDefaultResult(), column);
        public IReadOnlyList<string> ListStrings() => ListValues<string>();
        public IReadOnlyList<string> ListStrings(string column) => ListValues<string>(column);
        public IReadOnlyList<string?> ListNullableStrings() => Results().MapReadOnly(d => RowClass<string>(d));
        public IReadOnlyList<string?> ListNullableStrings(string column) {
            return Results().MapReadOnly(d => RowClass<string>(d, column));
        }

        public async Task<string> FirstStringAsync() => await FirstValueAsync<string>();
        public async Task<string> FirstStringAsync(string column) => await FirstValueAsync<string>(column);
        public async Task<string?> FirstOrDefaultStringAsync() => RowClass<string>(await FirstOrDefaultResultAsync());
        public async Task<string?> FirstOrDefaultStringAsync(string column) {
            return RowClass<string>(await FirstOrDefaultResultAsync(), column);
        }
        public async Task<string> SingleStringAsync() => await SingleValueAsync<string>();
        public async Task<string> SingleStringAsync(string column) => await SingleValueAsync<string>(column);
        public async Task<string?> SingleOrDefaultStringAsync() => RowClass<string>(await SingleOrDefaultResultAsync());
        public async Task<string?> SingleOrDefaultStringAsync(string column) {
            return RowClass<string>(await SingleOrDefaultResultAsync(), column);
        }
        public async Task<IReadOnlyList<string>> ListStringsAsync() => await ListValuesAsync<string>();
        public async Task<IReadOnlyList<string>> ListStringsAsync(string column) => await ListValuesAsync<string>(column);
        public async Task<IReadOnlyList<string?>> ListNullableStringsAsync() {
            return (await ResultsAsync()).MapReadOnly(d => RowClass<string>(d));
        }
        public async Task<IReadOnlyList<string?>> ListNullableStringsAsync(string column) {
            return (await ResultsAsync()).MapReadOnly(d => RowClass<string>(d, column));
        }

        public T FirstValue<T>() => RowValue<T>(FirstResult());
        public T FirstValue<T>(string column) => RowValue<T>(FirstResult(), column);
        public T? FirstOrDefaultValue<T>() where T : struct => RowStruct<T>(FirstOrDefaultResult());
        public T? FirstOrDefaultValue<T>(string column) where T : struct => RowStruct<T>(FirstOrDefaultResult(), column);
        public T SingleValue<T>()  => RowValue<T>(SingleResult());
        public T SingleValue<T>(string column) => RowValue<T>(SingleResult(), column);
        public T? SingleOrDefaultValue<T>() where T : struct => RowStruct<T>(SingleOrDefaultResult());
        public T? SingleOrDefaultValue<T>(string column) where T : struct => RowStruct<T>(SingleOrDefaultResult(), column);
        public IReadOnlyList<T> ListValues<T>() {
            return Results().MapReadOnly(d => RowValue<T>(d));
        }
        public IReadOnlyList<T> ListValues<T>(string column) {
            return Results().MapReadOnly(d => RowValue<T>(d, column));
        }
        public IReadOnlyList<T?> ListNullableValues<T>() where T : struct {
            return Results().MapReadOnly(d => RowStruct<T>(d));
        }
        public IReadOnlyList<T?> ListNullableValues<T>(string column) where T : struct {
            return Results().MapReadOnly(d => RowStruct<T>(d, column));
        }

        public async Task<T> FirstValueAsync<T>() {
            return RowValue<T>(await FirstResultAsync());
        }
        public async Task<T> FirstValueAsync<T>(string column) {
            return RowValue<T>(await FirstResultAsync(), column);
        }
        public async Task<T?> FirstOrDefaultValueAsync<T>() where T : struct {
            return RowStruct<T>(await FirstOrDefaultResultAsync());
        }
        public async Task<T?> FirstOrDefaultValueAsync<T>(string column) where T : struct {
            return RowStruct<T>(await FirstOrDefaultResultAsync(), column);
        }
        public async Task<T> SingleValueAsync<T>() {
            return RowValue<T>(await SingleResultAsync());
        }
        public async Task<T> SingleValueAsync<T>(string column) {
            return RowValue<T>(await SingleResultAsync(), column);
        }
        public async Task<T?> SingleOrDefaultValueAsync<T>() where T : struct {
            return RowStruct<T>(await SingleOrDefaultResultAsync());
        }
        public async Task<T?> SingleOrDefaultValueAsync<T>(string column) where T : struct {
            return RowStruct<T>(await SingleOrDefaultResultAsync(), column);
        }
        public async Task<IReadOnlyList<T>> ListValuesAsync<T>() {
            return (await ResultsAsync()).MapReadOnly(d => RowValue<T>(d));
        }
        public async Task<IReadOnlyList<T>> ListValuesAsync<T>(string column) {
            return (await ResultsAsync()).MapReadOnly(d => RowValue<T>(d, column));
        }
        public async Task<IReadOnlyList<T?>> ListNullableValuesAsync<T>() where T : struct {
            return (await ResultsAsync()).MapReadOnly(d => RowStruct<T>(d));
        }
        public async Task<IReadOnlyList<T?>> ListNullableValuesAsync<T>(string column) where T : struct {
            return (await ResultsAsync()).MapReadOnly(d => RowStruct<T>(d, column));
        }

        public T First<T>() where T : new() => List<T>().First();
        public T? FirstOrDefault<T>() where T : new() => List<T>().FirstOrDefault();
        public T Single<T>() where T : new() => List<T>().Single();
        public T? SingleOrDefault<T>() where T : new() => List<T>().SingleOrDefault();
        public IReadOnlyList<T> List<T>() where T : new() {
            return Results().MapReadOnly(d => ComponentModelHelper.ToObject<T>(d, _options.ColumnFormat));
        }

        public async Task<T> FirstAsync<T>() where T : new() => (await ListAsync<T>()).First();
        public async Task<T?> FirstOrDefaultAsync<T>() where T : new() => (await ListAsync<T>()).FirstOrDefault();
        public async Task<T> SingleAsync<T>() where T : new() => (await ListAsync<T>()).Single();
        public async Task<T?> SingleOrDefaultAsync<T>() where T : new() => (await ListAsync<T>()).SingleOrDefault();
        public async Task<IReadOnlyList<T>> ListAsync<T>() where T : new() {
            return (await ResultsAsync()).MapReadOnly(d => ComponentModelHelper.ToObject<T>(d, _options.ColumnFormat));
        }

        public SqlBuilderResultRow FirstResult() => Results().First();
        public SqlBuilderResultRow? FirstOrDefaultResult() => Results().FirstOrDefault();
        public SqlBuilderResultRow SingleResult() => Results().Single();
        public SqlBuilderResultRow? SingleOrDefaultResult() => Results().SingleOrDefault();
        public IReadOnlyList<SqlBuilderResultRow> Results() {
            string parameterizedSql = ToParameterizedSql();
            try {
                return _options.SqlFlavor.ToResults(parameterizedSql, _query.Parameters);
            }
            catch (Exception e) {
                throw new SqlExecutionException(e, _options.AddParameterizedSqlToException ? parameterizedSql : null);
            }
        }

        public async Task<SqlBuilderResultRow> FirstResultAsync() => (await ResultsAsync()).First();
        public async Task<SqlBuilderResultRow?> FirstOrDefaultResultAsync() => (await ResultsAsync()).FirstOrDefault();
        public async Task<SqlBuilderResultRow> SingleResultAsync() => (await ResultsAsync()).Single();
        public async Task<SqlBuilderResultRow?> SingleOrDefaultResultAsync() => (await ResultsAsync()).SingleOrDefault();
        public async Task<IReadOnlyList<SqlBuilderResultRow>> ResultsAsync() {
            string parameterizedSql = ToParameterizedSql();
            try {
                return await _options.SqlFlavor.ToResultsAsync(parameterizedSql, _query.Parameters);
            }
            catch (Exception e) {
                throw new SqlExecutionException(e, _options.AddParameterizedSqlToException ? parameterizedSql : null);
            }
        }

        public Pagination<T> Paginate<T>(long page, int pageSize) where T : new() {
            return Pagination<T>.Paginate(this, page, pageSize);
        }
        public async Task<Pagination<T>> PaginateAsync<T>(long page, int pageSize) where T : new() {
            return await Pagination<T>.PaginateAsync(this, page, pageSize);
        }

        public bool Execute() {
            string parameterizedSql = ToParameterizedSql();
            try {
                return _options.SqlFlavor.Execute(parameterizedSql, _query.Parameters);
            }
            catch (Exception e) {
                throw new SqlExecutionException(e, _options.AddParameterizedSqlToException ? parameterizedSql : null);
            }
        }

        public async Task<bool> ExecuteAsync() {
            string parameterizedSql = ToParameterizedSql();
            try {
                return await _options.SqlFlavor.ExecuteAsync(parameterizedSql, _query.Parameters);
            }
            catch (Exception e) {
                throw new SqlExecutionException(e, _options.AddParameterizedSqlToException ? parameterizedSql : null);
            }
        }

        public override string ToString() => ToParameterizedSql();

        public string ToUnsafeSql() {
            string query = ToParameterizedSql();
            foreach (var (key, value) in _query.Parameters) {
                if (value is string stringValue) {
                    query = query.Replace(key, $"'{value}'");
                }
                else {
                    query = query.Replace(key, value?.ToString() ?? "null");
                }
            }
            return query;
        }

        public string ToParameterizedSql() {
            string query = _query.ToParameterizedSql();
            int semiColonIndex = query.IndexOf(';');
            if (_options.OverprotectiveSqlInjection
                    && (semiColonIndex >= 0 && semiColonIndex != query.Length - 1 || query.Contains("--"))) {
                throw new PotentialSqlInjectionException();
            }
            return query;
        }

        public ICompleteQueryBuilder Clone() => new QueryBuilder(_options.Clone(), _query.Clone());
        public ICompleteQueryBuilder CloneWithoutSelect() {
            var query = _query.Clone();
            query.SelectColumns.Clear();
            return new QueryBuilder(_options.Clone(), query);
        }

        private T RowValue<T>(SqlBuilderResultRow row) => RowValue<T>(row, row.Keys.First());
        private T RowValue<T>(SqlBuilderResultRow row, string key) {
            return (T?)row[key] ?? throw new InvalidCastException($"The value for {key} is null");
        }
        private T? RowStruct<T>(SqlBuilderResultRow? row) where T : struct => RowStruct<T>(row, row?.Keys.First() ?? "");
        private T? RowStruct<T>(SqlBuilderResultRow? row, string key) where T : struct => (T?)row?[key];
        private T? RowClass<T>(SqlBuilderResultRow? row) where T : class => RowClass<T>(row, row?.Keys.First() ?? "");
        private T? RowClass<T>(SqlBuilderResultRow? row, string key) where T : class => (T?)row?[key];
    }

    public partial class QueryBuilder : ICompleteQueryBuilder { }
}