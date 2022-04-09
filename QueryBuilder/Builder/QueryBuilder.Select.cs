using SqlQueryBuilder.Model;

namespace SqlQueryBuilder.Builder;

public sealed partial class QueryBuilder : ISelectQueryBuilder, IGroupByQueryBuilder {
    public ISelectQueryBuilder ColumnAs(string column, string alias) => AddSelectColumn(null, column, alias);
    public ISelectQueryBuilder Column(string column) => AddSelectColumn(null, column);

    public ISelectQueryBuilder CountAllAs(string alias) => CountAs("*", alias);
    public ISelectQueryBuilder CountAllAs(string table, string alias) => CountAs(table + ".*", alias);
    public ISelectQueryBuilder CountAll() => Count("*");
    public ISelectQueryBuilder CountAll(string table) => Count(table + ".*");

    public ISelectQueryBuilder CountAs(string column, string alias) => AddSelectColumn("count", column, alias);
    public ISelectQueryBuilder Count(string column) => AddSelectColumn("count", column);

    public ISelectQueryBuilder SumAs(string column, string alias) => AddSelectColumn("sum", column, alias);
    public ISelectQueryBuilder Sum(string column) => AddSelectColumn("sum", column);

    public ISelectQueryBuilder AvgAs(string column, string alias) => AddSelectColumn("avg", column, alias);
    public ISelectQueryBuilder Avg(string column) => AddSelectColumn("avg", column);

    public ISelectQueryBuilder MinAs(string column, string alias) => AddSelectColumn("min", column, alias);
    public ISelectQueryBuilder Min(string column) => AddSelectColumn("min", column);

    public ISelectQueryBuilder MaxAs(string column, string alias) => AddSelectColumn("max", column, alias);
    public ISelectQueryBuilder Max(string column) => AddSelectColumn("max", column);

    private ISelectQueryBuilder AddSelectColumn(string? function, string column, string? alias = null) {
        _query.SelectColumns.Add(new Column(function, column, alias));
        return this;
    }
    public ISelectQueryBuilder SelectSubqueryAs(string alias, SubQueryFunc queryFunc) => SubquerySelect(alias, queryFunc);
    public ISelectQueryBuilder SelectSubquery(SubQueryFunc queryFunc) => SubquerySelect(null, queryFunc);

    private ISelectQueryBuilder SubquerySelect(string? alias, SubQueryFunc queryFunc) {
        var queryBuilder = new QueryBuilder(_options);
        queryFunc(queryBuilder);
        _query.SelectColumns.Add(new ColumnSubquery(alias, queryBuilder));
        return this;
    }

    public IQueryBuilder FromAs(string table, string alias) {
        _query.SelectFrom.Add(new Table(table, alias));
        return this;
    }
    public IQueryBuilder From(string table) {
        _query.SelectFrom.Add(new Table(table));
        return this;
    }

    public IQueryBuilder FromAs(string alias, SubQueryFunc queryFunc) => SubqueryFrom(alias, queryFunc);
    public IQueryBuilder From(SubQueryFunc queryFunc) => SubqueryFrom(null, queryFunc);

    private IQueryBuilder SubqueryFrom(string? alias, SubQueryFunc queryFunc) {
        var queryBuilder = new QueryBuilder(_options);
        queryFunc(queryBuilder);
        _query.SelectFrom.Add(new TableSubquery(alias, queryBuilder));
        return this;
    }

    public ISelectQueryBuilder RawColumn(string queryPart, params object?[] parameters) {
        _query.SelectColumns.Add(new RawQueryPart(queryPart, parameters));
        return this;
    }
}
