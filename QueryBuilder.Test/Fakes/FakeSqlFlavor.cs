using SqlQueryBuilder.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlQueryBuilder.Test.Fakes;

public sealed class FakeSqlFlavor : ISqlFlavor {
    // --- Implementation of the fake ---
    public List<string> ExecutedQueries { get; } = new List<string>();

    public object? NextSingleResult {
        set => NextResult = value is null ? null : new List<object?>() { value };
    }
    public IReadOnlyList<object?>? NextResult { get; set; }

    // --- Implementation of the interface ---
    public Task<bool> ExecuteAsync(string query, IDictionary<string, object?> parameters) {
        return Task.FromResult(Execute(query, parameters));
    }

    public bool Execute(string query, IDictionary<string, object?> parameters) {
        ExecutedQueries.Add(query);
        return true;
    }

    public Task<IReadOnlyList<T>> ExecuteWithResultsAsync<T>(string query, IDictionary<string, object?> parameters) {
        return Task.FromResult(ExecuteWithResults<T>(query, parameters));
    }

    public IReadOnlyList<T> ExecuteWithResults<T>(string query, IDictionary<string, object?> parameters) {
        ExecutedQueries.Add(query);
        return NextResult?.Cast<T>().ToList().AsReadOnly() ?? new List<T>().AsReadOnly();
    }

    public Task<ISqlTransactionFlavor> BeginTransactionAsync() => Task.FromResult(BeginTransaction());

    public ISqlTransactionFlavor BeginTransaction() => throw new System.NotImplementedException();

    public string WrapFieldName(string fieldName) => $"`{fieldName}`";

    public (string queryPart, object?[] parameters) Skip(long skipOffset) {
        return ("skip ?", new object?[] { skipOffset });
    }

    public (string queryPart, object?[] parameters) Take(int takeLimit) {
        return ("take ?", new object?[] { takeLimit });
    }
}
