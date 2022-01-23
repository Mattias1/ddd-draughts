namespace SqlQueryBuilder.Model;

internal interface IOrderBy : IQueryPart { }

internal readonly struct OrderBy : IOrderBy {
    public enum OrderDirection { Asc, Desc, None };

    public string ColumnName { get; }
    public OrderDirection Direction { get; }

    public OrderBy(string name, OrderDirection direction) {
        ColumnName = name;
        Direction = direction;
    }

    public void AppendToQuery(Query query, bool isFirst) {
        string directionString = Direction switch {
            OrderDirection.Asc => " asc",
            OrderDirection.Desc => " desc",
            _ => ""
        };

        if (!isFirst) {
            query.Builder.Append(", ");
        }
        query.Builder.Append(query.WrapField(ColumnName)).Append(directionString);
    }
}
