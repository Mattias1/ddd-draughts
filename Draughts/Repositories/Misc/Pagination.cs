using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Repositories.Misc;

public sealed class Pagination<T> {
    public IReadOnlyList<T> Results { get; }
    public long Count { get; }
    public long PageIndex { get; }
    public int PageSize { get; }

    public long Page => PageIndex + 1;
    public long PageCount => Count / PageSize + (Count % PageSize == 0 ? 0 : 1);
    public long BeginInclusive => Math.Min(PageIndex * PageSize + 1, Count);
    public long EndInclusive => Math.Min(Page * PageSize, Count);

    public Pagination(IReadOnlyList<T> results, long count, long pageIndex, int pageSize) {
        Results = results;
        Count = count;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }

    public Pagination<TMap> Map<TMap>(Func<T, TMap> func) {
        var results = Results.Select(func).ToList().AsReadOnly();
        return new Pagination<TMap>(results, Count, PageIndex, PageSize);
    }
}
