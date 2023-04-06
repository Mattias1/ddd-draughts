using SqlQueryBuilder.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlQueryBuilder.Builder;

public sealed partial class QueryBuilder : IQueryBuilderResult {
    public int SingleInt() => Single<int>();
    public IReadOnlyList<int> ListInts() => List<int>();
    public async Task<int> SingleIntAsync() => await SingleAsync<int>();
    public async Task<IReadOnlyList<int>> ListIntsAsync() => await ListAsync<int>();

    public long SingleLong() => Single<long>();
    public IReadOnlyList<long> ListLongs() => List<long>();
    public async Task<long> SingleLongAsync() => await SingleAsync<long>();
    public async Task<IReadOnlyList<long>> ListLongsAsync() => await ListAsync<long>();

    public string SingleString() => Single<string>();
    public IReadOnlyList<string> ListStrings() => List<string>();
    public async Task<string> SingleStringAsync() => await SingleAsync<string>();
    public async Task<IReadOnlyList<string>> ListStringsAsync() => await ListAsync<string>();

    public string? SingleOrDefaultString() => ListNullableStrings().SingleOrDefault();
    public async Task<string?> SingleOrDefaultStringAsync() => (await ListNullableStringsAsync()).SingleOrDefault();
    public IReadOnlyList<string?> ListNullableStrings() => List<string?>();
    public async Task<IReadOnlyList<string?>> ListNullableStringsAsync() => await ListAsync<string?>();

    public T Single<T>() => List<T>().Single();
    public T? SingleOrDefault<T>() => List<T>().SingleOrDefault();
    public async Task<T> SingleAsync<T>() => (await ListAsync<T>()).Single();
    public async Task<T?> SingleOrDefaultAsync<T>() => (await ListAsync<T>()).SingleOrDefault();

    public Pagination<T> Paginate<T>(long page, int pageSize) {
        return Pagination<T>.Paginate(this, page, pageSize);
    }
    public async Task<Pagination<T>> PaginateAsync<T>(long page, int pageSize) {
        return await Pagination<T>.PaginateAsync(this, page, pageSize);
    }
}
