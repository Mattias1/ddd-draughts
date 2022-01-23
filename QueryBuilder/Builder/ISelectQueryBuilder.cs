using static SqlQueryBuilder.Builder.QueryBuilder;

namespace SqlQueryBuilder.Builder;

public interface ISelectQueryBuilder : ISelectColumnsQueryBuilder, IFromQueryBuilder { }

public interface ISelectColumnsQueryBuilder : IQueryBuilderResult, IFromQueryBuilder {
    ISelectQueryBuilder ColumnAs(string column, string alias);
    ISelectQueryBuilder Column(string column);

    ISelectQueryBuilder CountAllAs(string alias);
    ISelectQueryBuilder CountAllAs(string table, string alias);
    ISelectQueryBuilder CountAll();
    ISelectQueryBuilder CountAll(string table);
    ISelectQueryBuilder CountAs(string column, string alias);
    ISelectQueryBuilder Count(string column);

    ISelectQueryBuilder SumAs(string column, string alias);
    ISelectQueryBuilder Sum(string column);

    ISelectQueryBuilder AvgAs(string column, string alias);
    ISelectQueryBuilder Avg(string column);

    ISelectQueryBuilder MinAs(string column, string alias);
    ISelectQueryBuilder Min(string column);

    ISelectQueryBuilder MaxAs(string column, string alias);
    ISelectQueryBuilder Max(string column);

    ISelectQueryBuilder SelectSubquery(SubQueryFunc queryFunc);
    ISelectQueryBuilder SelectSubqueryAs(string alias, SubQueryFunc queryFunc);

    ISelectQueryBuilder RawColumn(string queryPart, params object?[] parameters);
}

public interface IFromQueryBuilder {
    IQueryBuilder FromAs(string table, string alias);
    IQueryBuilder From(string table);
    IQueryBuilder From(SubQueryFunc queryFunc);
    IQueryBuilder FromAs(string alias, SubQueryFunc queryFunc);
}
