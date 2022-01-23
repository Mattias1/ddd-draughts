namespace SqlQueryBuilder.Builder;

public interface IGroupByQueryBuilder : IQueryBuilderResult {
    IQueryBuilder GroupBy(params string[] columns);

    IUpdateQueryBuilder RawGroupBy(string queryPart, params object?[] parameters);
}
