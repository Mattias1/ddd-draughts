using SqlQueryBuilder.Builder;

namespace SqlQueryBuilder.Model;

internal readonly struct TableSubquery : ITable {
    public string? Alias { get; }
    public QueryBuilder QueryBuilder { get; }

    public TableSubquery(string? alias, QueryBuilder queryBuilder) {
        Alias = alias;
        QueryBuilder = queryBuilder;
    }

    public void AppendToQuery(Query query, bool isFirst) {
        if (!isFirst) {
            query.Builder.Append(", ");
        }

        query.Builder.Append("(");
        query.AppendSubquery(QueryBuilder);
        query.Builder.Append(')');

        if (Alias is not null) {
            query.Builder.Append(" as ").Append(query.WrapField(Alias));
        }
    }
}
