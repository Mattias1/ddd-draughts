using System;
using System.Linq;

namespace SqlQueryBuilder.Model;

internal interface ILimit : IQueryPart { }

internal struct RawQueryPart : IWhere, IColumn, IJoin, IOrderBy, ILimit, IInsertValue, ISetColumn {
    public string QueryPart { get; }
    public object?[] Values { get; }

    public RawQueryPart(string query, params object?[] parameters) {
        QueryPart = query;
        Values = parameters;
    }

    public void AppendToQuery(Query query, bool isFirst) {
        var parts = QueryPart.Split('?');
        if (parts.Length - 1 != Values.Length) {
            throw new InvalidOperationException($"You do not have a matching amount of parameters {QueryPart}, " +
                string.Join(", ", Values.Select(p => p?.ToString() ?? "null")));
        }

        for (int i = 0; i < parts.Length - 1; i++) {
            query.Builder.Append(parts[i]);
            query.AppendParameter(Values[i]);
        }
        query.Builder.Append(parts[^1]);
    }
}
