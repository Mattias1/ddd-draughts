namespace SqlQueryBuilder.Builder {
    public interface ILimitQueryBuilder : IQueryBuilderBase {
        IQueryBuilder Skip(long offset);
        IQueryBuilder Take(int limit);

        IUpdateQueryBuilder RawLimit(string queryPart, params object?[] parameters);
    }
}
