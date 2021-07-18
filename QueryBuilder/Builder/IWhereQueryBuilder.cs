using static SqlQueryBuilder.Builder.QueryBuilder;

namespace SqlQueryBuilder.Builder {
    public interface IWhereQueryBuilder : IQueryBuilderBase {
        IQueryBuilder Where(QueryFunction queryFunc);
        IQueryBuilder And(QueryFunction queryFunc);

        IComparisonQueryBuilder Where(string column);
        IComparisonQueryBuilder And(string column);

        IQueryBuilder OrWhere(QueryFunction queryFunc);
        IQueryBuilder Or(QueryFunction queryFunc);

        IComparisonQueryBuilder OrWhere(string column);
        IComparisonQueryBuilder Or(string column);

        IQueryBuilder WhereNot(QueryFunction queryFunc);
        IQueryBuilder AndNot(QueryFunction queryFunc);

        IQueryBuilder OrWhereNot(QueryFunction queryFunc);
        IQueryBuilder OrNot(QueryFunction queryFunc);

        IQueryBuilder RawWhere(string queryPart, params object?[] parameters);

        IQueryBuilder WithoutWhere();
    }
}