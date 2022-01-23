namespace SqlQueryBuilder.Model;

internal readonly struct WhereIn : IWhere {
    public Where.WhereType WhereType { get; }
    public string ColumnName { get; }
    public string Operator { get; }
    public object?[] Values { get; }

    public WhereIn(Where.WhereType whereType, string columnName, string @operator, object?[] values) {
        WhereType = whereType;
        ColumnName = columnName;
        Operator = @operator;
        Values = values;
    }

    public void AppendToQuery(Query query, bool isFirst) {
        query.Builder.Append(' ').Append(Where.WhereTypeToString(WhereType, isFirst))
            .Append(' ').Append(query.WrapField(ColumnName))
            .Append(' ').Append(Operator)
            .Append(" (");

        bool isFirstParameter = true;
        foreach (var value in Values) {
            if (!isFirstParameter) {
                query.Builder.Append(", ");
            }
            query.AppendParameter(value);
            isFirstParameter = false;
        }

        query.Builder.Append(')');
    }
}
