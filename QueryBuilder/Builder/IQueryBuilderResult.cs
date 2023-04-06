using SqlQueryBuilder.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlQueryBuilder.Builder;

public interface ICompleteQueryBuilder : IQueryBuilder, IComparisonQueryBuilder,
    IInitialQueryBuilder, IInsertQueryBuilder, IUpdateQueryBuilder, ISelectQueryBuilder { }

public interface IQueryBuilder : IFromQueryBuilder, IJoinQueryBuilder, IWhereQueryBuilder,
    IGroupByQueryBuilder, IHavingQueryBuilder, IOrderQueryBuilder, ILimitQueryBuilder, IQueryBuilderResult { }

public interface IQueryBuilderResult : IQueryBuilderBase {
    int SingleInt();
    IReadOnlyList<int> ListInts();
    Task<int> SingleIntAsync();
    Task<IReadOnlyList<int>> ListIntsAsync();

    long SingleLong();
    IReadOnlyList<long> ListLongs();
    Task<long> SingleLongAsync();
    Task<IReadOnlyList<long>> ListLongsAsync();

    string SingleString();
    IReadOnlyList<string> ListStrings();
    Task<string> SingleStringAsync();
    Task<IReadOnlyList<string>> ListStringsAsync();

    string? SingleOrDefaultString();
    IReadOnlyList<string?> ListNullableStrings();
    Task<string?> SingleOrDefaultStringAsync();
    Task<IReadOnlyList<string?>> ListNullableStringsAsync();

    T Single<T>();
    T? SingleOrDefault<T>();
    Task<T> SingleAsync<T>();
    Task<T?> SingleOrDefaultAsync<T>();

    Pagination<T> Paginate<T>(long page, int pageSize);
    Task<Pagination<T>> PaginateAsync<T>(long page, int pageSize);
}
