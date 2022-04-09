using SqlQueryBuilder.Model;

namespace SqlQueryBuilder.Builder;

public sealed partial class QueryBuilder : IOrderQueryBuilder {
    public IQueryBuilder OrderByAsc(params string[] columns) {
        for (int i = 0; i < columns.Length - 1; i++) {
            _query.OrderByList.Add(new OrderBy(columns[i], OrderBy.OrderDirection.None));
        }
        if (columns.Length > 0) {
            _query.OrderByList.Add(new OrderBy(columns[^1], OrderBy.OrderDirection.Asc));
        }
        return this;
    }

    public IQueryBuilder OrderByDesc(params string[] columns) {
        for (int i = 0; i < columns.Length - 1; i++) {
            _query.OrderByList.Add(new OrderBy(columns[i], OrderBy.OrderDirection.None));
        }
        if (columns.Length > 0) {
            _query.OrderByList.Add(new OrderBy(columns[^1], OrderBy.OrderDirection.Desc));
        }
        return this;
    }

    public IUpdateQueryBuilder RawOrderBy(string queryPart, params object?[] parameters) {
        _query.OrderByList.Add(new RawQueryPart(queryPart, parameters));
        return this;
    }
}
