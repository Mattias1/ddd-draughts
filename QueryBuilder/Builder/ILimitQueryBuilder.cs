namespace SqlQueryBuilder.Builder;

public interface ILimitQueryBuilder : IQueryBuilderResult {
    IQueryBuilder Skip(long offset);
    IQueryBuilder Take(int limit);

    IUpdateQueryBuilder RawLimit(string queryPart, params object?[] parameters);
}
