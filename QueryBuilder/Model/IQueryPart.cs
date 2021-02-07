namespace SqlQueryBuilder.Model {
    internal interface IQueryPart {
        public void AppendToQuery(Query query, bool isFirst);
    }
}
