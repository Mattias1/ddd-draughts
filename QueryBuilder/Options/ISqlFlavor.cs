using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlQueryBuilder.Options {
    public interface ISqlFlavor {
        Task<bool> ExecuteAsync(string query, IDictionary<string, object?> parameters);
        bool Execute(string query, IDictionary<string, object?> parameters);
        Task<IReadOnlyList<SqlBuilderResultRow>> ToResultsAsync(string query, IDictionary<string, object?> parameters);
        IReadOnlyList<SqlBuilderResultRow> ToResults(string query, IDictionary<string, object?> parameters);

        Task<ISqlTransactionFlavor> BeginTransactionAsync();
        ISqlTransactionFlavor BeginTransaction();

        (string queryPart, object?[] parameters) Skip(long skipOffset);
        (string queryPart, object?[] parameters) Take(int takeLimit);
    }
}
