namespace SqlQueryBuilder.Model;

internal interface ISetColumn : IQueryPart { }

internal readonly struct SetColumn : ISetColumn {
    public enum ValueType { Any, Column }

    public string ColumnName { get; }
    public object? Value { get; }
    public ValueType Type { get; }

    public SetColumn(string name, object? value, ValueType type = ValueType.Any) {
        ColumnName = name;
        Value = value;
        Type = type;
    }

    public void AppendToQuery(Query query, bool isFirst) {
        if (!isFirst) {
            query.Builder.Append(", ");
        }
        query.Builder.Append(query.WrapField(ColumnName)).Append(" = ");
        if (Type == ValueType.Column && Value is string columnValue) {
            query.Builder.Append(query.WrapField(columnValue));
        } else {
            query.AppendParameter(Value);
        }
    }
}
