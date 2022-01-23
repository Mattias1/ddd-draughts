namespace SqlQueryBuilder.Model;

internal interface IQueryPart {
    void AppendToQuery(Query query, bool isFirst);
}
