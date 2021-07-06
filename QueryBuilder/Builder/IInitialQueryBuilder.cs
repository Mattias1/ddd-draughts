namespace SqlQueryBuilder.Builder {
    public interface IInitialQueryBuilder : IQueryBuilderBase {
        IQueryBuilder SelectAllFrom(string table);
        IQueryBuilder SelectAllFromAs(string table, string alias);
        ISelectQueryBuilder SelectAll();
        ISelectQueryBuilder SelectAll(string table);
        ISelectQueryBuilder SelectDistinct();
        ISelectQueryBuilder Select(params string[] columns);
        ISelectQueryBuilder Select();

        IInsertQueryBuilder InsertInto(string table);

        IUpdateQueryBuilder Update(string table);

        IQueryBuilder DeleteFrom(string table);
        IQueryBuilder DeleteFromAs(string table, string alias);

        ICompleteQueryBuilder Raw(string query, params object?[] parameters);
    }
}