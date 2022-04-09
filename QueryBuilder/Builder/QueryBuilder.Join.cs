using SqlQueryBuilder.Model;

namespace SqlQueryBuilder.Builder;

public sealed partial class QueryBuilder : IJoinQueryBuilder {
    public IQueryBuilder Join(string table, string leftColumn, string rightColumn) {
        return JoinBase("join", table, leftColumn, rightColumn);
    }

    public IQueryBuilder JoinAs(string table, string alias, string leftColumn, string rightColumn) {
        return JoinBaseAlias("join", table, alias, leftColumn, rightColumn);
    }

    public IQueryBuilder LeftJoin(string table, string leftColumn, string rightColumn) {
        return JoinBase("left join", table, leftColumn, rightColumn);
    }

    public IQueryBuilder LeftJoinAs(string table, string alias, string leftColumn, string rightColumn) {
        return JoinBaseAlias("left join", table, alias, leftColumn, rightColumn);
    }

    public IQueryBuilder RightJoin(string table, string leftColumn, string rightColumn) {
        return JoinBase("right join", table, leftColumn, rightColumn);
    }

    public IQueryBuilder RightJoinAs(string table, string alias, string leftColumn, string rightColumn) {
        return JoinBaseAlias("right join", table, alias, leftColumn, rightColumn);
    }

    public IQueryBuilder FullJoin(string table, string leftColumn, string rightColumn) {
        return JoinBase("full join", table, leftColumn, rightColumn);
    }

    public IQueryBuilder FullJoinAs(string table, string alias, string leftColumn, string rightColumn) {
        return JoinBaseAlias("full join", table, alias, leftColumn, rightColumn);
    }

    public IQueryBuilder CrossJoin(string table, string leftColumn, string rightColumn) {
        return JoinBase("cross join", table, leftColumn, rightColumn);
    }

    public IQueryBuilder CrossJoinAs(string table, string alias, string leftColumn, string rightColumn) {
        return JoinBaseAlias("cross join", table, alias, leftColumn, rightColumn);
    }

    private IQueryBuilder JoinBase(string keyword, string table, string leftColumn, string rightColumn) {
        _query.Joins.Add(new Join(keyword, table, leftColumn, "=", rightColumn));
        return this;
    }

    private IQueryBuilder JoinBaseAlias(string keyword, string table, string alias, string leftColumn, string rightColumn) {
        _query.Joins.Add(new Join(keyword, table, alias, leftColumn, "=", rightColumn));
        return this;
    }

    public ICompleteQueryBuilder RawJoin(string queryPart, params object?[] parameters) {
        _query.Joins.Add(new RawQueryPart(queryPart, parameters));
        return this;
    }
}
