using SqlQueryBuilder.Common;
using SqlQueryBuilder.Model;
using System.Collections.Generic;
using System.Linq;
using static SqlQueryBuilder.Model.SetColumn;

namespace SqlQueryBuilder.Builder;

public sealed partial class QueryBuilder : IUpdateQueryBuilder {
    public IUpdateQueryBuilder SetColumn(string column, object? value) {
        _query.UpdateValues.Add(new SetColumn(column, value));
        return this;
    }

    public IUpdateQueryBuilder SetColumnToColumn(string column, string columnValue) {
        _query.UpdateValues.Add(new SetColumn(column, columnValue, ValueType.Column));
        return this;
    }

    public IUpdateQueryBuilder RawSet(string queryPart, params object?[] parameters) {
        _query.UpdateValues.Add(new RawQueryPart(queryPart, parameters));
        return this;
    }

    public IUpdateQueryBuilder SetFrom<T>(T model) where T : notnull {
        return SetFromDictionary(ComponentModelHelper.ToDictionary(model, _options.ColumnFormat));
    }
    public IUpdateQueryBuilder SetWithoutIdFrom<T>(T model) where T : notnull {
        return SetFromDictionary(ComponentModelHelper.ToDictionary(model, _options.ColumnFormat), "id");
    }

    public IUpdateQueryBuilder SetWithoutColumnsFrom<T>(T model, params string[] columns) where T : notnull {
        return SetFromDictionary(ComponentModelHelper.ToDictionary(model, _options.ColumnFormat), columns);
    }

    public IUpdateQueryBuilder SetFromDictionary(
            IReadOnlyDictionary<string, object?> dictionary, params string[] ignoreKeys) {
        foreach (var (key, value) in dictionary) {
            if (!ignoreKeys.Contains(key)) {
                SetColumn(key, value);
            }
        }
        return this;
    }
}
