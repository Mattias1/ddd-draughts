using SqlQueryBuilder.Common;
using SqlQueryBuilder.Model;
using System.Collections.Generic;
using System.Linq;

namespace SqlQueryBuilder.Builder;

public sealed partial class QueryBuilder : IInsertQueryBuilder {
    public IInsertQueryBuilder Columns(params string[] columns) {
        foreach (string c in columns) {
            _query.InsertColumns.Add(new Column(c));
        }
        return this;
    }

    public IInsertQueryBuilder Values(params object?[] parameters) => Values(parameters.AsEnumerable());
    public IInsertQueryBuilder Values<T>(IEnumerable<T> parameters) {
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

    public IQueryBuilderResult InsertFrom<T>(params T[] models) where T : notnull => InsertFrom(models.AsEnumerable());
    public IQueryBuilderResult InsertFrom<T>(IEnumerable<T> models) where T : notnull {
        var dictionaries = models.Select(m => ComponentModelHelper.ToDictionary(m, _options.ColumnFormat)).ToArray();
        if (dictionaries.Length > 0) {
            InsertFromDictionary(dictionaries.First());
        }
        var values = dictionaries.Skip(1).SelectMany(d => d.Values).Select(v => (IInsertValue)new InsertValue(v));
        _query.InsertValues.AddRange(values);
        return this;
    }

    public IQueryBuilderResult InsertFromDictionary(IReadOnlyDictionary<string, object?> dictionary) {
        foreach (var (key, value) in dictionary) {
            _query.InsertColumns.Add(new Column(key));
            _query.InsertValues.Add(new InsertValue(value));
        }
        return this;
    }
}
