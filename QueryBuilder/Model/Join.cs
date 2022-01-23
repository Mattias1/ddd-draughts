namespace SqlQueryBuilder.Model;

internal interface IJoin : IQueryPart { }

internal readonly struct Join : IJoin {
    public string Keyword { get; }
    public Table Table { get; }
    public string LeftColumn { get; }
    public string Operator { get; }
    public string RightColumn { get; }

    public Join(string keyword, string table, string left, string @operator, string right)
        : this(keyword, table, null, left, @operator, right) { }
    public Join(string keyword, string table, string? alias, string left, string @operator, string right) {
        Keyword = keyword;
        Table = new Table(table, alias);
        LeftColumn = left;
        Operator = @operator;
        RightColumn = right;
    }

    public void AppendToQuery(Query query, bool isFirst) {
        query.Builder
            .Append(' ').Append(Keyword)
            .Append(' ');
        Table.AppendToQuery(query, true);
        query.Builder
            .Append(" on ").Append(query.WrapField(LeftColumn))
            .Append(' ').Append(Operator)
            .Append(' ').Append(query.WrapField(RightColumn));
    }
}
