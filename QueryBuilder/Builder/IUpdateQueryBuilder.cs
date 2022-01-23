using System.Collections.Generic;

namespace SqlQueryBuilder.Builder;

public interface IUpdateQueryBuilder : IQueryBuilder {
    IUpdateQueryBuilder SetColumn(string column, object? value);
    IUpdateQueryBuilder SetColumnToColumn(string column, string columnValue);

    IUpdateQueryBuilder RawSet(string queryPart, params object?[] parameters);

    IUpdateQueryBuilder SetFrom<T>(T model) where T : notnull;
    IUpdateQueryBuilder SetWithoutIdFrom<T>(T model) where T : notnull;
    IUpdateQueryBuilder SetWithoutColumnsFrom<T>(T model, params string[] columns) where T : notnull;
    IUpdateQueryBuilder SetFromDictionary(IReadOnlyDictionary<string, object?> dictionary, params string[] ignoreKeys);
}
