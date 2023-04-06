using SqlQueryBuilder.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlQueryBuilder.Options;

public sealed class Pagination<T> {
    public const string INVALID_COUNT_ERROR = "Cannot get the count of this query.";

    public IReadOnlyList<T> Results { get; }
    public long Count { get; }
    public long PageIndex { get; }
    public int PageSize { get; }

    public long Page => PageIndex + 1;
    public long PageCount => Count / PageSize + (Count % PageSize == 0 ? 0 : 1);
    public long BeginInclusive => PageIndex * PageSize + 1;
    public long EndInclusive => BeginInclusive + PageSize;

    public Pagination(IReadOnlyList<T> results, long count, long pageIndex, int pageSize) {
        Results = results;
        Count = count;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    public Pagination<TMap> Map<TMap>(Func<T, TMap> func) where TMap : new() {
        var results = Results.Select(func).ToList().AsReadOnly();
        return new Pagination<TMap>(results, Count, PageIndex, PageSize);
    }

    public static Pagination<T> Paginate(IQueryBuilderResult query, long page, int pageSize) {
        long? count = query.CloneWithoutSelect().CountAll().SingleLong();
        if (count is null) {
            throw new InvalidOperationException(INVALID_COUNT_ERROR);
        }

        long pageIndex = GetPageIndex(page);

        var results = query.Cast()
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .List<T>();
        return new Pagination<T>(results, count.Value, pageIndex, pageSize);
    }

    public static async Task<Pagination<T>> PaginateAsync(IQueryBuilderResult query, long page, int pageSize) {
        long? count = await query.CloneWithoutSelect().CountAll().SingleLongAsync();
        if (count is null) {
            throw new InvalidOperationException(INVALID_COUNT_ERROR);
        }

        long pageIndex = GetPageIndex(page);

        var results = await query.Cast()
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ListAsync<T>();
        return new Pagination<T>(results, count.Value, pageIndex, pageSize);
    }

    private static long GetPageIndex(long page) => Math.Max(page - 1, 0);
}
