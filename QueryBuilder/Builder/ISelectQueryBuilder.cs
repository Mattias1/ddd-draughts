namespace SqlQueryBuilder.Builder {
    public interface ISelectQueryBuilder : ISelectColumnsQueryBuilder, IFromQueryBuilder { }

    public interface ISelectColumnsQueryBuilder : IQueryBuilderBase, IFromQueryBuilder {
        ISelectQueryBuilder ColumnAs(string column, string alias);
        ISelectQueryBuilder Column(string column);

        ISelectQueryBuilder CountAllAs(string alias);
        ISelectQueryBuilder CountAll();
        ISelectQueryBuilder CountAs(string column, string alias);
        ISelectQueryBuilder Count(string column);

        ISelectQueryBuilder SumAs(string column, string alias);
        ISelectQueryBuilder Sum(string column);

        ISelectQueryBuilder AvgAs(string column, string alias);
        ISelectQueryBuilder Avg(string column);

        ISelectQueryBuilder MinAs(string column, string alias);
        ISelectQueryBuilder Min(string column);

        ISelectQueryBuilder MaxAs(string column, string alias);
        ISelectQueryBuilder Max(string column);

        ISelectQueryBuilder RawColumn(string queryPart, params object?[] parameters);
    }

    public interface IFromQueryBuilder {
        IQueryBuilder FromAs(string table, string alias);
        IQueryBuilder From(string table);
    }
}