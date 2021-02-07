namespace SqlQueryBuilder.Builder {
    public interface IGroupByQueryBuilder : IQueryBuilderBase {
        IQueryBuilder GroupBy(params string[] columns);

        IUpdateQueryBuilder RawGroupBy(string queryPart, params object?[] parameters);
    }
}