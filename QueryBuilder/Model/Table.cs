namespace SqlQueryBuilder.Model;

internal interface ITable : IQueryPart { }

internal readonly struct Table : ITable {
    public string TableName { get; }
    public string? Alias { get; }

    public Table(string name, string? alias = null) {
        TableName = name;
        Alias = alias;
    }

    public void AppendToQuery(Query query, bool isFirst) {
        if (!isFirst) {
            query.Builder.Append(", ");
        }
        query.Builder.Append(query.WrapField(TableName));
        if (Alias is not null) {
            query.Builder.Append(" as ").Append(query.WrapField(Alias));
        }
    }
}
