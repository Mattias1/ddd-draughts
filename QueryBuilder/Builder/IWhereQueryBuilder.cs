using static SqlQueryBuilder.Builder.QueryBuilder;

namespace SqlQueryBuilder.Builder;

public interface IWhereQueryBuilder : IQueryBuilderResult {
    IQueryBuilder Where(SubWhereFunc queryFunc);
    IQueryBuilder And(SubWhereFunc queryFunc);

    IComparisonQueryBuilder Where(string column);
    IComparisonQueryBuilder And(string column);

    IQueryBuilder OrWhere(SubWhereFunc queryFunc);
    IQueryBuilder Or(SubWhereFunc queryFunc);

    IComparisonQueryBuilder OrWhere(string column);
    IComparisonQueryBuilder Or(string column);

    IQueryBuilder WhereNot(SubWhereFunc queryFunc);
    IQueryBuilder AndNot(SubWhereFunc queryFunc);

    IQueryBuilder OrWhereNot(SubWhereFunc queryFunc);
    IQueryBuilder OrNot(SubWhereFunc queryFunc);

    IQueryBuilder WhereExists(SubQueryFunc queryFunc);
    IQueryBuilder WhereNotExists(SubQueryFunc queryFunc);

    IQueryBuilder RawWhere(string queryPart, params object?[] parameters);

    IQueryBuilder WithoutWhere();
}
