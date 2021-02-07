namespace SqlQueryBuilder.Builder {
    public interface IOrderQueryBuilder : IQueryBuilderBase {
        IQueryBuilder OrderByAsc(params string[] columns);
        IQueryBuilder OrderByDesc(params string[] columns);

        IUpdateQueryBuilder RawOrderBy(string queryPart, params object?[] parameters);
    }
}