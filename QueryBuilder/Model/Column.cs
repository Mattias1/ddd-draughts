namespace SqlQueryBuilder.Model;

internal interface IColumn : IQueryPart { }

internal readonly struct Column : IColumn {
    public string? Function { get; }
    public string ColumnName { get; }
    public string? Alias { get; }

    public Column(string name, string? alias = null) : this(null, name, alias) { }
    public Column(string? function, string name, string? alias = null) {
        Function = function;
        ColumnName = name;
        Alias = alias;
    }

    public void AppendToQuery(Query query, bool isFirst) {
        if (!isFirst) {
            query.Builder.Append(", ");
        }
        if (Function is null) {
            query.Builder.Append(query.WrapField(ColumnName));
        } else {
            query.Builder.Append(Function).Append('(').Append(query.WrapField(ColumnName)).Append(')');
        }
        if (Alias is not null) {
            query.Builder.Append(" as ").Append(query.WrapField(Alias));
        }
    }
}
