using SqlQueryBuilder.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SqlQueryBuilder.Options;

public class SqlBuilderResultRow : IReadOnlyDictionary<string, object?> {
    private Dictionary<string, Dictionary<string, object?>> _results;

    protected SqlBuilderResultRow() {
        _results = new Dictionary<string, Dictionary<string, object?>>();
    }

    public IReadOnlyList<string> TableNames() => _results.Keys.ToList().AsReadOnly();

    public IReadOnlyDictionary<string, object?> Single() {
        if (_results.Keys.Count != 1) {
            throw new SqlQueryBuilderException("Sql result contains results from multiple tables, "
                + "use Get(tableName) to get the results.");
        }
        return _results[_results.Keys.Single()];
    }

    public IReadOnlyDictionary<string, object?> Get(string tableName) {
        if (!_results.ContainsKey(tableName)) {
            throw new SqlQueryBuilderException($"Sql result does not contains results from table {tableName}.");
        }
        return _results[tableName];
    }

    public static SqlBuilderResultRow FromReader(DbDataReader reader) {
        var result = new SqlBuilderResultRow();
        var columnSchemas = reader.GetColumnSchema();
        for (int i = 0; i < reader.FieldCount; i++) {
            result.Add(columnSchemas[i].BaseTableName ?? "", reader.GetName(i), reader.GetValue(i));
        }
        return result;
    }

    protected void Add(string table, string column, object? value) {
        if (!_results.ContainsKey(table)) {
            _results[table] = new Dictionary<string, object?>();
        }
        _results[table][column] = ParseBasics(value);
    }

    private static object? ParseBasics(object? value) {
        if (value == DBNull.Value) {
            return null;
        }
        return value;
    }

    // --- IReadOnlyDictionary implementation ---
    public object? this[string key] => Single()[key];

    public IEnumerable<string> Keys => Single().Keys;

    public IEnumerable<object?> Values => Single().Values;

    public int Count => Single().Count;

    public bool ContainsKey(string key) => Single().ContainsKey(key);

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator() => Single().GetEnumerator();

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object? value) => Single().TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => Single().GetEnumerator();
}
