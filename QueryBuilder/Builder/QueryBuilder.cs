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

        internal QueryBuilder(QueryBuilderOptions options) {
            _query = new Query(options);
            _options = options;
        }

        private string ExtractAliasOrColumn(string column) {
            int index = column.LastIndexOf(" ");
            return index >= 0 ? column.Substring(index + 1) : column;
        }

        public int? FirstInt(string column) => FirstValue<int>(column);
        public int? FirstInt() => FirstValue<int>();
        public int? SingleInt(string column) => SingleValue<int>();
        public int? SingleInt() => SingleValue<int>();
        public async Task<int?> FirstIntAsync(string column) => await FirstValueAsync<int>(column);
        public async Task<int?> FirstIntAsync() => await FirstValueAsync<int>();
        public async Task<int?> SingleIntAsync(string column) => await SingleValueAsync<int>(column);
        public async Task<int?> SingleIntAsync() => await SingleValueAsync<int>();

        public long? FirstLong(string column) => FirstValue<long>(column);
        public long? FirstLong() => FirstValue<long>();
        public long? SingleLong(string column) => SingleValue<long>(column);
        public long? SingleLong() => SingleValue<long>();
        public async Task<long?> FirstLongAsync(string column) => await FirstValueAsync<long>(column);
        public async Task<long?> FirstLongAsync() => await FirstValueAsync<long>();
        public async Task<long?> SingleLongAsync(string column) => await SingleValueAsync<long>(column);
        public async Task<long?> SingleLongAsync() => await SingleValueAsync<long>();

        public T? FirstValue<T>(string column) where T : struct => (T?)FirstResult()[column];
        public T? FirstValue<T>() where T : struct {
            var dict = FirstResult();
            var key = dict.Keys.First();
            return (T?)dict[key];
        }
        public T? SingleValue<T>(string column) where T : struct => (T?)SingleResult()[column];
        public T? SingleValue<T>() where T : struct {
            var dict = SingleResult();
            var key = dict.Keys.Single();
            return (T?)dict[key];
        }

        public async Task<T?> FirstValueAsync<T>(string column) where T : struct => (T?)(await FirstResultAsync())[column];
        public async Task<T?> FirstValueAsync<T>() where T : struct {
            var dict = await FirstResultAsync();
            var key = dict.Keys.First();
            return (T?)dict[key];
        }
        public async Task<T?> SingleValueAsync<T>(string column) where T : struct => (T?)(await SingleResultAsync())[column];
        public async Task<T?> SingleValueAsync<T>() where T : struct {
            var dict = await SingleResultAsync();
            var key = dict.Keys.Single();
            return (T?)dict[key];
        }

        public string? FirstString(string column) => (string?)FirstResult()[column];
        public string? FirstString()  {
            var dict = FirstResult();
            var key = dict.Keys.First();
            return (string?)dict[key];
        }
        public string? SingleString(string column) => (string?)SingleResult()[column];
        public string? SingleString()  {
            var dict = SingleResult();
            var key = dict.Keys.Single();
            return (string?)dict[key];
        }

        public async Task<string?> FirstStringAsync(string column) => (string?)(await FirstResultAsync())[column];
        public async Task<string?> FirstStringAsync() {
            var dict = await FirstResultAsync();
            var key = dict.Keys.First();
            return (string?)dict[key];
        }
        public async Task<string?> SingleStringAsync(string column) => (string?)(await SingleResultAsync())[column];
        public async Task<string?> SingleStringAsync() {
            var dict = await SingleResultAsync();
            var key = dict.Keys.Single();
            return (string?)dict[key];
        }

        public T First<T>() where T : new() => List<T>().First();
        public T? FirstOrDefault<T>() where T : new() => List<T>().FirstOrDefault();
        public T Single<T>() where T : new() => List<T>().Single();
        public T? SingleOrDefault<T>() where T : new() => List<T>().SingleOrDefault();
        public IReadOnlyList<T> List<T>() where T : new() {
            return Results().Select(d => ComponentModelHelper.ToObject<T>(d, _options.ColumnFormat)).ToList();
        }

        public async Task<T> FirstAsync<T>() where T : new() => (await ListAsync<T>()).First();
        public async Task<T?> FirstOrDefaultAsync<T>() where T : new() => (await ListAsync<T>()).FirstOrDefault();
        public async Task<T> SingleAsync<T>() where T : new() => (await ListAsync<T>()).Single();
        public async Task<T?> SingleOrDefaultAsync<T>() where T : new() => (await ListAsync<T>()).SingleOrDefault();
        public async Task<IReadOnlyList<T>> ListAsync<T>() where T : new() {
            return (await ResultsAsync()).Select(d => ComponentModelHelper.ToObject<T>(d, _options.ColumnFormat)).ToList();
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
            if (_options.OverprotectiveSqlInjection && (query.Contains(';') || query.Contains("--"))) {
                throw PotentialSqlInjectionException.ForDangerousCharacters();
            }
            return query;
        }
    }

    public partial class QueryBuilder : ICompleteQueryBuilder { }
}