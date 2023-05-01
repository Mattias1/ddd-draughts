using SqlQueryBuilder.Builder;
using SqlQueryBuilder.MySql;
using SqlQueryBuilder.Options;
using System;

namespace Draughts.Repositories.Misc;

public sealed class DbContext {
    private const string SERVER = "localhost";
    private static DbContext? _instance = null;
    public static DbContext Get => _instance ?? throw new InvalidOperationException("The database context isn't initialized yet.");

    private readonly DbConnectionInfo _userDb, _authDb, _gameDb, _miscDb;

    private DbContext(int dbPort, string dbPassword) {
        _userDb = new DbConnectionInfo("draughts_user", dbPort, "draughts_user", dbPassword);
        _authDb = new DbConnectionInfo("draughts_auth", dbPort, "draughts_auth", dbPassword);
        _gameDb = new DbConnectionInfo("draughts_game", dbPort, "draughts_game", dbPassword);
        _miscDb = new DbConnectionInfo("draughts_misc", dbPort, "draughts_game", dbPassword); // All users can access the misc DB.
    }

    public ISqlTransactionFlavor BeginUserTransaction() => _userDb.Connection().BeginTransaction();
    public ISqlTransactionFlavor BeginAuthTransaction() => _authDb.Connection().BeginTransaction();
    public ISqlTransactionFlavor BeginGameTransaction() => _gameDb.Connection().BeginTransaction();
    public ISqlTransactionFlavor BeginMiscTransaction() => _miscDb.Connection().BeginTransaction();

    public IInitialQueryBuilder Query(ISqlTransactionFlavor transaction) => QueryBuilder.Init(transaction);

    public static void Init(int dbPort, string dbPassword) => _instance = new DbContext(dbPort, dbPassword);

    private record DbConnectionInfo(string Database, int Port, string User, string Password) {
        public MySqlFlavor Connection() => new MySqlFlavor(SERVER, Port, User, Password, Database);
    }
}
