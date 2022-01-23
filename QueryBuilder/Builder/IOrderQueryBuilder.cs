namespace SqlQueryBuilder.Builder;

public interface IOrderQueryBuilder : IQueryBuilderResult {
    IQueryBuilder OrderByAsc(params string[] columns);
    IQueryBuilder OrderByDesc(params string[] columns);

    IUpdateQueryBuilder RawOrderBy(string queryPart, params object?[] parameters);
}
