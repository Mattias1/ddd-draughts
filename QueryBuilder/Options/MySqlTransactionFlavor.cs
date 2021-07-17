using MySql.Data.MySqlClient;
using SqlQueryBuilder.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SqlQueryBuilder.Options {
    public class MySqlTransactionFlavor : ISqlTransactionFlavor {
        private readonly MySqlConnection _connection;
        private readonly MySqlTransaction _transaction;

        private MySqlTransactionFlavor(MySqlConnection connection, MySqlTransaction transaction) {
            _connection = connection;
            _transaction = transaction;
        }

        public async Task<bool> ExecuteAsync(string query, IDictionary<string, object?> parameters) {
            var cmd = BuildCommand(query, parameters);
            int nrOfRowsAffected = await cmd.ExecuteNonQueryAsync();
            return nrOfRowsAffected > 0;
        }
        public bool Execute(string query, IDictionary<string, object?> parameters) {
            var cmd = BuildCommand(query, parameters);
            int nrOfRowsAffected = cmd.ExecuteNonQuery();
            return nrOfRowsAffected > 0;
        }

        public async Task<IReadOnlyList<SqlBuilderResultRow>> ToResultsAsync(string query, IDictionary<string, object?> parameters) {
            var cmd = BuildCommand(query, parameters);

            var results = new List<SqlBuilderResultRow>();
            using (var reader = await cmd.ExecuteReaderAsync()) {
                while (await reader.ReadAsync()) {
                    results.Add(SqlBuilderResultRow.FromReader(reader));
                }
            }
            return results.AsReadOnly();
        }
        public IReadOnlyList<SqlBuilderResultRow> ToResults(string query, IDictionary<string, object?> parameters) {
            var cmd = BuildCommand(query, parameters);

            var results = new List<SqlBuilderResultRow>();
            using (var reader = cmd.ExecuteReader()) {
                while (reader.Read()) {
                    results.Add(SqlBuilderResultRow.FromReader(reader));
                }
            }
            return results.AsReadOnly();
        }

        private MySqlCommand BuildCommand(string query, IDictionary<string, object?> parameters) {
            var cmd = new MySqlCommand(query, _connection, _transaction);
            foreach (var (key, value) in parameters) {
                cmd.Parameters.AddWithValue(key, value);
            }
            cmd.Prepare();
            return cmd;
        }

        public Task<ISqlTransactionFlavor> BeginTransactionAsync() {
            throw new InvalidOperationException("The transaction is already started.");
        } 
        public ISqlTransactionFlavor BeginTransaction() {
            throw new InvalidOperationException("The transaction is already started.");
        } 

        public async Task CommitAsync() => await _transaction.CommitAsync();
        public void Commit() => _transaction.Commit();

        public async Task RollbackAsync() => await _transaction.RollbackAsync();
        public void Rollback() => _transaction.Rollback();

        public void Dispose() {
            _transaction.Dispose();
            _connection.Dispose();
        }

        public static async Task<MySqlTransactionFlavor> BeginTransactionAsync(MySqlConnection connection) {
            try {
                await connection.OpenAsync();
                var transaction = await connection.BeginTransactionAsync();
                return new MySqlTransactionFlavor(connection, transaction);
            }
            catch (Exception e) {
                throw new SqlQueryBuilderException(e.Message, e);
            }
        }
        public static MySqlTransactionFlavor BeginTransaction(MySqlConnection connection) {
            try {
                connection.Open();
                var transaction = connection.BeginTransaction();
                return new MySqlTransactionFlavor(connection, transaction);
            }
            catch (Exception e) {
                throw new SqlQueryBuilderException(e.Message, e);
            }
        }

        public string WrapFieldName(string fieldName) => MySqlFlavor.WrapFieldNameStatic(fieldName);
        public (string queryPart, object?[] parameters) Skip(long skipOffset) => MySqlFlavor.SkipStatic(skipOffset);
        public (string queryPart, object?[] parameters) Take(int takeLimit) => MySqlFlavor.TakeStatic(takeLimit);
    }
}
