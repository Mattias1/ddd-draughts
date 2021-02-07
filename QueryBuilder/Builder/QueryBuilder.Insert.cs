using SqlQueryBuilder.Common;
using SqlQueryBuilder.Model;
using System.Collections.Generic;
using System.Linq;

namespace SqlQueryBuilder.Builder {
    public partial class QueryBuilder : IInsertQueryBuilder {
        public IInsertQueryBuilder Columns(params string[] columns) {
            foreach (string c in columns) {
                _query.InsertColumns.Add(new Column(c));
            }
            return this;
        }

        public IQueryBuilderBase Values(params object?[] parameters) => Values(parameters.AsEnumerable());
        public IQueryBuilderBase Values<T>(IEnumerable<T> parameters) {
            foreach (var p in parameters) {
                _query.InsertValues.Add(new InsertValue(p));
            }
            return this;
        }

        public IInsertQueryBuilder RawInsertColumn(string queryPart, params object?[] parameters) {
            _query.InsertColumns.Add(new RawQueryPart(queryPart, parameters));
            return this;
        }

        public ICompleteQueryBuilder RawInsertValue(string queryPart, params object?[] parameters) {
            _query.InsertValues.Add(new RawQueryPart(queryPart, parameters));
            return this;
        }

        public IQueryBuilderBase InsertFrom<T>(T model) where T : notnull {
            return InsertFromDictionary(ComponentModelHelper.ToDictionary(model, _options.ColumnFormat));
        }

        public IQueryBuilderBase InsertFromDictionary(IReadOnlyDictionary<string, object?> dictionary) {
            foreach (var (key, value) in dictionary) {
                _query.InsertColumns.Add(new Column(key));
                _query.InsertValues.Add(new InsertValue(value));
            }
            return this;
        }
    }
}
