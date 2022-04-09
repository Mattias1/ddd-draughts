using SqlQueryBuilder.Model;

namespace SqlQueryBuilder.Builder;

public sealed partial class QueryBuilder : IGroupByQueryBuilder {
    public IQueryBuilder GroupBy(params string[] columns) {
        foreach (string c in columns) {
            _query.GroupByColumns.Add(new Column(c));
        }
        return this;
    }

    public IUpdateQueryBuilder RawGroupBy(string queryPart, params object?[] parameters) {
        _query.GroupByColumns.Add(new RawQueryPart(queryPart, parameters));
        return this;
    }
}
