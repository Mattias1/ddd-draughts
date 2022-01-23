namespace SqlQueryBuilder.Builder;

public interface IJoinQueryBuilder : IQueryBuilderResult {
    IQueryBuilder Join(string table, string leftColumn, string rightColumn);
    IQueryBuilder JoinAs(string table, string alias, string leftColumn, string rightColumn);
    IQueryBuilder LeftJoin(string table, string leftColumn, string rightColumn);
    IQueryBuilder LeftJoinAs(string table, string alias, string leftColumn, string rightColumn);
    IQueryBuilder RightJoin(string table, string leftColumn, string rightColumn);
    IQueryBuilder RightJoinAs(string table, string alias, string leftColumn, string rightColumn);
    IQueryBuilder FullJoin(string table, string leftColumn, string rightColumn);
    IQueryBuilder FullJoinAs(string table, string alias, string leftColumn, string rightColumn);
    IQueryBuilder CrossJoin(string table, string leftColumn, string rightColumn);
    IQueryBuilder CrossJoinAs(string table, string alias, string leftColumn, string rightColumn);

    ICompleteQueryBuilder RawJoin(string queryPart, params object?[] parameters);
}
