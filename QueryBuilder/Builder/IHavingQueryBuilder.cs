using static SqlQueryBuilder.Builder.QueryBuilder;

namespace SqlQueryBuilder.Builder {
    public interface IHavingQueryBuilder : IQueryBuilderResult {
        IQueryBuilder Having(QueryFunction queryFunc);
        IQueryBuilder AndHaving(QueryFunction queryFunc);

        IComparisonQueryBuilder Having(string column);
        IComparisonQueryBuilder AndHaving(string column);

        IQueryBuilder OrHaving(QueryFunction queryFunc);

        IComparisonQueryBuilder OrHaving(string column);

        IQueryBuilder NotHaving(QueryFunction queryFunc);
        IQueryBuilder AndNotHaving(QueryFunction queryFunc);

        IQueryBuilder OrNotHaving(QueryFunction queryFunc);

        IQueryBuilder RawHaving(string queryPart, params object?[] parameters);
    }
}