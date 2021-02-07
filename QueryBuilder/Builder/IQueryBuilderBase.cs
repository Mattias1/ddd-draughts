using SqlQueryBuilder.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlQueryBuilder.Builder {
    public interface ICompleteQueryBuilder : IQueryBuilder, IComparisonQueryBuilder,
        IInitialQueryBuilder, IInsertQueryBuilder, IUpdateQueryBuilder, ISelectQueryBuilder { }

    public interface IQueryBuilder : IFromQueryBuilder, IJoinQueryBuilder, IWhereQueryBuilder,
        IGroupByQueryBuilder, IHavingQueryBuilder, IOrderQueryBuilder, IQueryBuilderBase { }

    public interface IQueryBuilderBase {
        T First<T>() where T : new();
        T FirstOrDefault<T>() where T : new();
        T Single<T>() where T : new();
        T SingleOrDefault<T>() where T : new();
        IReadOnlyList<T> List<T>() where T : new();

        Task<T> FirstAsync<T>() where T : new();
        Task<T> FirstOrDefaultAsync<T>() where T : new();
        Task<T> SingleAsync<T>() where T : new();
        Task<T> SingleOrDefaultAsync<T>() where T : new();
        Task<IReadOnlyList<T>> ListAsync<T>() where T : new();

        int? FirstInt(string column);
        int? FirstInt();
        int? SingleInt(string column);
        int? SingleInt();
        Task<int?> FirstIntAsync(string column);
        Task<int?> FirstIntAsync();
        Task<int?> SingleIntAsync(string column);
        Task<int?> SingleIntAsync();

        long? FirstLong(string column);
        long? FirstLong();
        long? SingleLong(string column);
        long? SingleLong();
        Task<long?> FirstLongAsync(string column);
        Task<long?> FirstLongAsync();
        Task<long?> SingleLongAsync(string column);
        Task<long?> SingleLongAsync();

        T? FirstValue<T>(string column) where T : struct;
        T? FirstValue<T>() where T : struct;
        T? SingleValue<T>(string column) where T : struct;
        T? SingleValue<T>() where T : struct;
        Task<T?> FirstValueAsync<T>(string column) where T : struct;
        Task<T?> FirstValueAsync<T>() where T : struct;
        Task<T?> SingleValueAsync<T>(string column) where T : struct;
        Task<T?> SingleValueAsync<T>() where T : struct;

        string? FirstString(string column);
        string? FirstString();
        string? SingleString(string column);
        string? SingleString();
        Task<string?> FirstStringAsync(string column);
        Task<string?> FirstStringAsync();
        Task<string?> SingleStringAsync(string column);
        Task<string?> SingleStringAsync();

        SqlBuilderResultRow FirstResult();
        SqlBuilderResultRow FirstOrDefaultResult();
        SqlBuilderResultRow SingleResult();
        SqlBuilderResultRow SingleOrDefaultResult();
        IReadOnlyList<SqlBuilderResultRow> Results();

        Task<SqlBuilderResultRow> FirstResultAsync();
        Task<SqlBuilderResultRow> FirstOrDefaultResultAsync();
        Task<SqlBuilderResultRow> SingleResultAsync();
        Task<SqlBuilderResultRow> SingleOrDefaultResultAsync();
        Task<IReadOnlyList<SqlBuilderResultRow>> ResultsAsync();

        bool Execute();
        Task<bool> ExecuteAsync();

        string ToString();
        string ToUnsafeSql();
        string ToParameterizedSql();
    }
}