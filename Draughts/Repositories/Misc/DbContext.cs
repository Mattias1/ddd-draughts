using SqlQueryBuilder.Builder;
using SqlQueryBuilder.MySql;
using SqlQueryBuilder.Options;
using System;

namespace Draughts.Repositories.Misc;

public sealed class DbContext {
    public const string AUTH_DATABASE = "draughts_auth";
    public const string GAME_DATABASE = "draughts_game";
    public const string MISC_DATABASE = "draughts_misc";
    public const string USER_DATABASE = "draughts_user";

    private static DbContext? _instance = null;
    public static DbContext Get => _instance ?? throw new InvalidOperationException("The database context isn't initialized yet.");

    private readonly DbConnectionInfo _userDb, _authDb, _gameDb, _miscDb;

    private DbContext(string server, int port, string password) {
        _userDb = new DbConnectionInfo("draughts_user", server, port, "draughts_user", password);
        _authDb = new DbConnectionInfo("draughts_auth", server, port, "draughts_auth", password);
        _gameDb = new DbConnectionInfo("draughts_game", server, port, "draughts_game", password);
        _miscDb = new DbConnectionInfo("draughts_misc", server, port, "draughts_game", password); // All users can access the misc DB.
    }

    public ISqlTransactionFlavor BeginUserTransaction() => _userDb.Connection().BeginTransaction();
    public ISqlTransactionFlavor BeginAuthTransaction() => _authDb.Connection().BeginTransaction();
    public ISqlTransactionFlavor BeginGameTransaction() => _gameDb.Connection().BeginTransaction();
    public ISqlTransactionFlavor BeginMiscTransaction() => _miscDb.Connection().BeginTransaction();

    public IInitialQueryBuilder Query(ISqlTransactionFlavor transaction) => QueryBuilder.Init(transaction);

    public static void Init(string dbServer, int dbPort, string dbPassword) {
        _instance = new DbContext(dbServer, dbPort, dbPassword);
    }

    public string ConnectionStringForMigrations(string database) {
        return $"Server={_miscDb.Server};Port={_miscDb.Port};User Id=draughts_console;Password={_miscDb.Password};"
            + $"Database={database}";
    }

    private record DbConnectionInfo(string Database, string Server, int Port, string User, string Password) {
        public MySqlFlavor Connection() => new MySqlFlavor(Server, Port, User, Password, Database);
    }
}
