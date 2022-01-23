namespace SqlQueryBuilder.Model;

internal interface IInsertValue : IQueryPart { }

internal readonly struct InsertValue : IInsertValue {
    public object? Value { get; }

    public InsertValue(object? value) {
        Value = value;
    }

    public void AppendToQuery(Query query, bool isFirst) {
        if (!isFirst) {
            query.Builder.Append(", ");
        }
        query.AppendParameter(Value);
    }
}
