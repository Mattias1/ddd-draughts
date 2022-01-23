using SqlQueryBuilder.Builder;

namespace SqlQueryBuilder.Model;

internal readonly struct WhereSubQuery : IWhere {
    public Where.WhereType WhereType { get; }
    public string? ColumnName { get; }
    public string Operator { get; }
    public QueryBuilder QueryBuilder { get; }

    public WhereSubQuery(Where.WhereType whereType, string? columnName, string @operator, QueryBuilder queryBuilder) {
        WhereType = whereType;
        ColumnName = columnName;
        Operator = @operator;
        QueryBuilder = queryBuilder;
    }

    public void AppendToQuery(Query query, bool isFirst) {
        query.Builder.Append(' ').Append(Where.WhereTypeToString(WhereType, isFirst));
        if (ColumnName is not null) {
            query.Builder.Append(' ').Append(query.WrapField(ColumnName));
        }
        query.Builder.Append(' ').Append(Operator);

        query.Builder.Append(" (");
        query.AppendSubquery(QueryBuilder);
        query.Builder.Append(')');
    }
}
