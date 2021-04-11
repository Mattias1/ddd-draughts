using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlQueryBuilder.Options {
    public class MySqlFlavor : ISqlFlavor {
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
            using (_connection) {
                await _connection.OpenAsync();
                var cmd = BuildCommand(query, parameters);
                int nrOfRowsAffected = await cmd.ExecuteNonQueryAsync();
                return nrOfRowsAffected > 0;
            }
        }

        public bool Execute(string query, IDictionary<string, object?> parameters) {
            using (_connection) {
                _connection.Open();
                var cmd = BuildCommand(query, parameters);
                int nrOfRowsAffected = cmd.ExecuteNonQuery();
                return nrOfRowsAffected > 0;
            }
        }

        public async Task<IReadOnlyList<SqlBuilderResultRow>> ToResultsAsync(
                string query, IDictionary<string, object?> parameters) {
            using (_connection) {
                await _connection.OpenAsync();
                var cmd = BuildCommand(query, parameters);

                var results = new List<SqlBuilderResultRow>();
                using (var reader = await cmd.ExecuteReaderAsync()) {
                    while (await reader.ReadAsync()) {
                        results.Add(SqlBuilderResultRow.FromReader(reader));
                    }
                }
                return results.AsReadOnly();
            }
        }

        public IReadOnlyList<SqlBuilderResultRow> ToResults(string query, IDictionary<string, object?> parameters) {
            using (_connection) {
                _connection.Open();
                var cmd = BuildCommand(query, parameters);

                var results = new List<SqlBuilderResultRow>();
                using (var reader = cmd.ExecuteReader()) {
                    while (reader.Read()) {
                        results.Add(SqlBuilderResultRow.FromReader(reader));
                    }
                }
                return results.AsReadOnly();
            }
        }

        private MySqlCommand BuildCommand(string query, IDictionary<string, object?> parameters) {
            var cmd = new MySqlCommand(query, _connection);
            foreach (var (key, value) in parameters) {
                cmd.Parameters.AddWithValue(key, value);
            }
            cmd.Prepare();
            return cmd;
        }

        public async Task<ISqlTransactionFlavor> BeginTransactionAsync() {
            return await MySqlTransactionFlavor.BeginTransactionAsync(_connection);
        }
        public ISqlTransactionFlavor BeginTransaction() => MySqlTransactionFlavor.BeginTransaction(_connection);

        public (string queryPart, object?[] parameters) Skip(long skipOffset) => SkipStatic(skipOffset);
        public (string queryPart, object?[] parameters) Take(int takeLimit) => TakeStatic(takeLimit);

        public static (string queryPart, object?[] parameters) SkipStatic(long skipOffset) {
            return ("offset ?", new object?[] { skipOffset });
        }
        public static (string queryPart, object?[] parameters) TakeStatic(int takeLimit) {
            return ("limit ?", new object?[] { takeLimit });
        }
    }
}
