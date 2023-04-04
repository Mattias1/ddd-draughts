using SqlQueryBuilder.Builder;
using SqlQueryBuilder.Options;
using System;
using System.Threading.Tasks;

namespace SqlQueryBuilder.IntegrationTest;

public sealed class DbContext {
    private const string SERVER = "localhost";
    private const int PORT = 53306;
    private const string USER = "sqlqb_it_user";
    private const string PASSWORD = "devapp";
    private const string DATABASE = "sqlquirybuilder_it_db";

    private static DbContext? _instance;
    public static DbContext Get => _instance ??= new DbContext(new MySqlFlavor(SERVER, PORT, USER, PASSWORD, DATABASE));

    private readonly ISqlFlavor _sqlFlavor;

    private DbContext(ISqlFlavor sqlFlavor) {
        _sqlFlavor = sqlFlavor;
    }

    public void WithTransaction(Action<ISqlTransactionFlavor> function) {
        using var transaction = BeginTransaction();
        function(transaction);
        transaction.Commit();
    }
    public T WithTransaction<T>(Func<ISqlTransactionFlavor, T> function) {
        using var transaction = BeginTransaction();
        var result = function(transaction);
        transaction.Commit();
        return result;
    }

    public async Task WithTransactionAsync(Func<ISqlTransactionFlavor, Task> function) {
        using var transaction = await BeginTransactionAsync();
        await function(transaction);
        await transaction.CommitAsync();
    }
    public async Task<T> WithTransactionAsync<T>(Func<ISqlTransactionFlavor, Task<T>> function) {
        using var transaction = await BeginTransactionAsync();
        var result = await function(transaction);
        await transaction.CommitAsync();
        return result;
    }

    public ISqlTransactionFlavor BeginTransaction() => _sqlFlavor.BeginTransaction();

    public async Task<ISqlTransactionFlavor> BeginTransactionAsync() => await _sqlFlavor.BeginTransactionAsync();

    public IInitialQueryBuilder QueryWithoutTransaction() => QueryBuilder.Init(_sqlFlavor);

    public IInitialQueryBuilder Query(ISqlTransactionFlavor transaction) => QueryBuilder.Init(transaction);
}
