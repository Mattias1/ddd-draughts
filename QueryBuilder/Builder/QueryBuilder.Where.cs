using SqlQueryBuilder.Model;
using System.Collections.Generic;
using static SqlQueryBuilder.Model.Where;

namespace SqlQueryBuilder.Builder {
    public partial class QueryBuilder : IWhereQueryBuilder {
        public delegate IQueryBuilder QueryFunction(IQueryBuilder builder);

        private (WhereType type, string column, List<IWhere> forest)? _preparedLeaf;

        public IQueryBuilder Where(QueryFunction queryFunc) => And(queryFunc);
        public IQueryBuilder And(QueryFunction queryFunc) => RecursiveWhere(WhereType.And, queryFunc);

        public IComparisonQueryBuilder Where(string column) => And(column);
        public IComparisonQueryBuilder And(string column) => PrepareWhereLeaf(WhereType.And, column);

        public IQueryBuilder OrWhere(QueryFunction queryFunc) => Or(queryFunc);
        public IQueryBuilder Or(QueryFunction queryFunc) => RecursiveWhere(WhereType.Or, queryFunc);

        public IComparisonQueryBuilder OrWhere(string column) => Or(column);
        public IComparisonQueryBuilder Or(string column) => PrepareWhereLeaf(WhereType.Or, column);

        public IQueryBuilder WhereNot(QueryFunction queryFunc) => AndNot(queryFunc);
        public IQueryBuilder AndNot(QueryFunction queryFunc) => RecursiveWhere(WhereType.AndNot, queryFunc);
        public IQueryBuilder OrWhereNot(QueryFunction queryFunc) => OrNot(queryFunc);
        public IQueryBuilder OrNot(QueryFunction queryFunc) => RecursiveWhere(WhereType.OrNot, queryFunc);

        public IQueryBuilder RawWhere(string queryPart, params object?[] parameters) {
            _query.WhereForest.Add(new RawQueryPart(queryPart, parameters));
            return this;
        }

        private IQueryBuilder RecursiveWhere(WhereType type, QueryFunction queryFunc) {
            var queryBuilder = new QueryBuilder(_options);
            queryFunc(queryBuilder);
            _query.WhereForest.Add(new Where(type, queryBuilder._query.WhereForest));
            return this;
        }

        private IComparisonQueryBuilder PrepareWhereLeaf(WhereType type, string column) {
            _preparedLeaf = (type, column, _query.WhereForest);
            return this;
        }

        public IQueryBuilder WithoutWhere() {
            _explicitlyWithoutWhere = true;
            return this;
        }
    }
}
