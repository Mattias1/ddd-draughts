using SqlQueryBuilder.Model;
using static SqlQueryBuilder.Model.Where;

namespace SqlQueryBuilder.Builder {
    public partial class QueryBuilder : IHavingQueryBuilder {
        public IQueryBuilder Having(QueryFunction queryFunc) => AndHaving(queryFunc);
        public IQueryBuilder AndHaving(QueryFunction queryFunc) => RecursiveHaving(WhereType.AndHaving, queryFunc);

        public IComparisonQueryBuilder Having(string column) => AndHaving(column);
        public IComparisonQueryBuilder AndHaving(string column) => PrepareHavingLeaf(WhereType.AndHaving, column);

        public IQueryBuilder OrHaving(QueryFunction queryFunc) => RecursiveHaving(WhereType.OrHaving, queryFunc);

        public IComparisonQueryBuilder OrHaving(string column) => PrepareHavingLeaf(WhereType.OrHaving, column);

        public IQueryBuilder NotHaving(QueryFunction queryFunc) => AndNotHaving(queryFunc);
        public IQueryBuilder AndNotHaving(QueryFunction queryFunc) => RecursiveHaving(WhereType.AndNotHaving, queryFunc);
        public IQueryBuilder OrNotHaving(QueryFunction queryFunc) => RecursiveHaving(WhereType.OrNotHaving, queryFunc);

        public IQueryBuilder RawHaving(string queryPart, params object?[] parameters) {
            _query.HavingForest.Add(new RawQueryPart(queryPart, parameters));
            return this;
        }

        private IQueryBuilder RecursiveHaving(WhereType type, QueryFunction queryFunc) {
            var queryBuilder = new QueryBuilder(_options);
            queryFunc(queryBuilder);
            _query.HavingForest.Add(new Where(type, queryBuilder._query.HavingForest));
            return this;
        }

        private IComparisonQueryBuilder PrepareHavingLeaf(WhereType type, string column) {
            _preparedLeaf = (type, column, _query.HavingForest);
            return this;
        }
    }
}
