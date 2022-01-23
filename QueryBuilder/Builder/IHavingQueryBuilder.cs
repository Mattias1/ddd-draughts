using static SqlQueryBuilder.Builder.QueryBuilder;

namespace SqlQueryBuilder.Builder;

public interface IHavingQueryBuilder : IQueryBuilderResult {
    IQueryBuilder Having(SubWhereFunc queryFunc);
    IQueryBuilder AndHaving(SubWhereFunc queryFunc);

    IComparisonQueryBuilder Having(string column);
    IComparisonQueryBuilder AndHaving(string column);

    IQueryBuilder OrHaving(SubWhereFunc queryFunc);

    IComparisonQueryBuilder OrHaving(string column);

    IQueryBuilder NotHaving(SubWhereFunc queryFunc);
    IQueryBuilder AndNotHaving(SubWhereFunc queryFunc);

    IQueryBuilder OrNotHaving(SubWhereFunc queryFunc);

    IQueryBuilder RawHaving(string queryPart, params object?[] parameters);
}
