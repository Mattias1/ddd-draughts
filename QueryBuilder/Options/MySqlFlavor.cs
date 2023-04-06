using Dapper;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SqlQueryBuilder.Options;

public sealed class MySqlFlavor : ISqlFlavor {
    private readonly MySqlConnection _connection;

    public MySqlFlavor(string server, string user, string password, string database)
        : this($"Server={server};User Id={user};Password={password};Database={database}") { }
    public MySqlFlavor(string server, int port, string user, string password, string database)
        : this($"Server={server};Port={port};User Id={user};Password={password};Database={database}") { }
    public MySqlFlavor(string connectionString) : this(new MySqlConnection(connectionString)) { }
    public MySqlFlavor(MySqlConnection connection) {
        _connection = connection;
    }

    public async Task<bool> ExecuteAsync(string query, IDictionary<string, object?> parameters) {
        await using (_connection) {
            int nrOfRowsAffected = await _connection.ExecuteAsync(query, parameters);
            return nrOfRowsAffected > 0;
        }
    }

    public bool Execute(string query, IDictionary<string, object?> parameters) {
        using (_connection) {
            int nrOfRowsAffected = _connection.Execute(query, parameters);
            return nrOfRowsAffected > 0;
        }
    }

    public async Task<IReadOnlyList<T>> ExecuteWithResultsAsync<T>(string query, IDictionary<string, object?> parameters) {
        await using (_connection) {
            var result = await _connection.QueryAsync<T>(query, new DynamicParameters(parameters));
            return result.ToList().AsReadOnly();
        }
    }
    public IReadOnlyList<T> ExecuteWithResults<T>(string query, IDictionary<string, object?> parameters) {
        using (_connection) {
            return _connection.Query<T>(query, new DynamicParameters(parameters)).ToList().AsReadOnly();
        }
    }

    public async Task<ISqlTransactionFlavor> BeginTransactionAsync() {
        return await MySqlTransactionFlavor.BeginTransactionAsync(_connection);
    }
    public ISqlTransactionFlavor BeginTransaction() => MySqlTransactionFlavor.BeginTransaction(_connection);

    public string WrapFieldName(string fieldName) => WrapFieldNameStatic(fieldName);
    public static string WrapFieldNameStatic(string fieldName) => $"`{fieldName}`";

    public (string queryPart, object?[] parameters) Skip(long skipOffset) => SkipStatic(skipOffset);
    public (string queryPart, object?[] parameters) Take(int takeLimit) => TakeStatic(takeLimit);

    public static (string queryPart, object?[] parameters) SkipStatic(long skipOffset) {
        return ("offset ?", new object?[] { skipOffset });
    }
    public static (string queryPart, object?[] parameters) TakeStatic(int takeLimit) {
        return ("limit ?", new object?[] { takeLimit });
    }
}
