using SqlQueryBuilder.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlQueryBuilder.Builder {
    public interface ICompleteQueryBuilder : IQueryBuilder, IComparisonQueryBuilder,
        IInitialQueryBuilder, IInsertQueryBuilder, IUpdateQueryBuilder, ISelectQueryBuilder { }

    public interface IQueryBuilder : IFromQueryBuilder, IJoinQueryBuilder, IWhereQueryBuilder,
        IGroupByQueryBuilder, IHavingQueryBuilder, IOrderQueryBuilder, ILimitQueryBuilder, IQueryBuilderBase { }

    public interface IQueryBuilderBase {
        ICompleteQueryBuilder Cast();

        int FirstInt();
        int FirstInt(string column);
        int SingleInt();
        int SingleInt(string column);
        IReadOnlyList<int> ListInts();
        IReadOnlyList<int> ListInts(string column);

        Task<int> FirstIntAsync();
        Task<int> FirstIntAsync(string column);
        Task<int> SingleIntAsync();
        Task<int> SingleIntAsync(string column);
        Task<IReadOnlyList<int>> ListIntsAsync();
        Task<IReadOnlyList<int>> ListIntsAsync(string column);

        long FirstLong();
        long FirstLong(string column);
        long SingleLong();
        long SingleLong(string column);
        IReadOnlyList<long> ListLongs();
        IReadOnlyList<long> ListLongs(string column);

        Task<long> FirstLongAsync();
        Task<long> FirstLongAsync(string column);
        Task<long> SingleLongAsync();
        Task<long> SingleLongAsync(string column);
        Task<IReadOnlyList<long>> ListLongsAsync();
        Task<IReadOnlyList<long>> ListLongsAsync(string column);

        string FirstString();
        string FirstString(string column);
        string? FirstOrDefaultString();
        string? FirstOrDefaultString(string column);
        string SingleString();
        string SingleString(string column);
        string? SingleOrDefaultString();
        string? SingleOrDefaultString(string column);
        IReadOnlyList<string> ListStrings();
        IReadOnlyList<string> ListStrings(string column);
        IReadOnlyList<string?> ListNullableStrings();
        IReadOnlyList<string?> ListNullableStrings(string column);

        Task<string> FirstStringAsync();
        Task<string> FirstStringAsync(string column);
        Task<string?> FirstOrDefaultStringAsync();
        Task<string?> FirstOrDefaultStringAsync(string column);
        Task<string> SingleStringAsync();
        Task<string> SingleStringAsync(string column);
        Task<string?> SingleOrDefaultStringAsync();
        Task<string?> SingleOrDefaultStringAsync(string column);
        Task<IReadOnlyList<string>> ListStringsAsync();
        Task<IReadOnlyList<string>> ListStringsAsync(string column);
        Task<IReadOnlyList<string?>> ListNullableStringsAsync();
        Task<IReadOnlyList<string?>> ListNullableStringsAsync(string column);

        T FirstValue<T>();
        T FirstValue<T>(string column);
        T? FirstOrDefaultValue<T>() where T : struct;
        T? FirstOrDefaultValue<T>(string column) where T : struct;
        T SingleValue<T>();
        T SingleValue<T>(string column);
        T? SingleOrDefaultValue<T>() where T : struct;
        T? SingleOrDefaultValue<T>(string column) where T : struct;
        IReadOnlyList<T> ListValues<T>();
        IReadOnlyList<T> ListValues<T>(string column);
        IReadOnlyList<T?> ListNullableValues<T>() where T : struct;
        IReadOnlyList<T?> ListNullableValues<T>(string column) where T : struct;

        Task<T> FirstValueAsync<T>();
        Task<T> FirstValueAsync<T>(string column);
        Task<T?> FirstOrDefaultValueAsync<T>() where T : struct;
        Task<T?> FirstOrDefaultValueAsync<T>(string column) where T : struct;
        Task<T> SingleValueAsync<T>();
        Task<T> SingleValueAsync<T>(string column);
        Task<T?> SingleOrDefaultValueAsync<T>() where T : struct;
        Task<T?> SingleOrDefaultValueAsync<T>(string column) where T : struct;
        Task<IReadOnlyList<T>> ListValuesAsync<T>();
        Task<IReadOnlyList<T>> ListValuesAsync<T>(string column);
        Task<IReadOnlyList<T?>> ListNullableValuesAsync<T>() where T : struct;
        Task<IReadOnlyList<T?>> ListNullableValuesAsync<T>(string column) where T : struct;

        T First<T>() where T : new();
        T? FirstOrDefault<T>() where T : new();
        T Single<T>() where T : new();
        T? SingleOrDefault<T>() where T : new();
        IReadOnlyList<T> List<T>() where T : new();

        Task<T> FirstAsync<T>() where T : new();
        Task<T?> FirstOrDefaultAsync<T>() where T : new();
        Task<T> SingleAsync<T>() where T : new();
        Task<T?> SingleOrDefaultAsync<T>() where T : new();
        Task<IReadOnlyList<T>> ListAsync<T>() where T : new();

        SqlBuilderResultRow FirstResult();
        SqlBuilderResultRow? FirstOrDefaultResult();
        SqlBuilderResultRow SingleResult();
        SqlBuilderResultRow? SingleOrDefaultResult();
        IReadOnlyList<SqlBuilderResultRow> Results();

        Task<SqlBuilderResultRow> FirstResultAsync();
        Task<SqlBuilderResultRow?> FirstOrDefaultResultAsync();
        Task<SqlBuilderResultRow> SingleResultAsync();
        Task<SqlBuilderResultRow?> SingleOrDefaultResultAsync();
        Task<IReadOnlyList<SqlBuilderResultRow>> ResultsAsync();

        Pagination<T> Paginate<T>(long page, int pageSize) where T : new();
        Task<Pagination<T>> PaginateAsync<T>(long page, int pageSize) where T : new();

        bool Execute();
        Task<bool> ExecuteAsync();

        string ToString();
        string ToUnsafeSql();
        string ToParameterizedSql();

        ICompleteQueryBuilder Clone();
        ICompleteQueryBuilder CloneWithoutSelect();
    }
}
