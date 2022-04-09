using SqlQueryBuilder.Model;

namespace SqlQueryBuilder.Builder;

public sealed partial class QueryBuilder : ILimitQueryBuilder {
    public IQueryBuilder Skip(long offset) {
        var (queryPart, parameters) = _options.SqlFlavor.Skip(offset);
        _query.Limits.Add(new RawQueryPart(' ' + queryPart, parameters));
        return this;
    }

    public IQueryBuilder Take(int limit) {
        var (queryPart, parameters) = _options.SqlFlavor.Take(limit);
        _query.Limits.Insert(0, new RawQueryPart(' ' + queryPart, parameters));
        return this;
    }

    public IUpdateQueryBuilder RawLimit(string queryPart, params object?[] parameters) {
        _query.Limits.Add(new RawQueryPart(queryPart, parameters));
        return this;
    }
}
