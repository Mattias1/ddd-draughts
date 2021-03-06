using SqlQueryBuilder.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlQueryBuilder.Test.Fakes {
    public class FakeSqlFlavor : ISqlFlavor {
        // --- Implementation of the fake ---
        public List<string> ExecutedQueries { get; } = new List<string>();

        public SqlBuilderResultRow? NextSingleResult {
            set => NextResult = value is null ? null : new List<SqlBuilderResultRow>() { value };
        }
        public IReadOnlyList<SqlBuilderResultRow>? NextResult { get; set; }

        // --- Implementation of the interface ---
        public Task<bool> ExecuteAsync(string query, IDictionary<string, object?> parameters) {
            return Task.FromResult(Execute(query, parameters));
        }

        public bool Execute(string query, IDictionary<string, object?> parameters) {
            ExecutedQueries.Add(query);
            return true;
        }

        public Task<IReadOnlyList<SqlBuilderResultRow>> ToResultsAsync(string query, IDictionary<string, object?> parameters) {
            return Task.FromResult(ToResults(query, parameters));
        }

        public IReadOnlyList<SqlBuilderResultRow> ToResults(string query, IDictionary<string, object?> parameters) {
            ExecutedQueries.Add(query);
            return NextResult ?? new List<SqlBuilderResultRow>();
        }

        public Task<ISqlTransactionFlavor> BeginTransactionAsync() => Task.FromResult(BeginTransaction());

        public ISqlTransactionFlavor BeginTransaction() => throw new System.NotImplementedException();

        public (string queryPart, object?[] parameters) Skip(long skipOffset) {
            return ("skip ?", new object?[] { skipOffset });
        }

        public (string queryPart, object?[] parameters) Take(int takeLimit) {
            return ("take ?", new object?[] { takeLimit });
        }
    }
}
