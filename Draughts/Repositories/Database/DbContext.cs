using SqlQueryBuilder.Builder;
using SqlQueryBuilder.Options;

namespace Draughts.Repositories.Database {
    public class DbContext {
        private const string SERVER = "localhost";
        private const int PORT = 52506;
        private static DbContext? _instance = null;
        public static DbContext Get => _instance ??= new DbContext();

        private readonly DbConnectionInfo _userDb;
        private readonly DbConnectionInfo _authUserDb;
        private readonly DbConnectionInfo _gameDb;
        private readonly DbConnectionInfo _miscDb;

        public DbContext() {
            _userDb = new DbConnectionInfo("draughts_user", "draughts_user", "devapp");
            _authUserDb = new DbConnectionInfo("draughts_authuser", "draughts_authuser", "devapp");
            _gameDb = new DbConnectionInfo("draughts_game", "draughts_game", "devapp");
            _miscDb = new DbConnectionInfo("draughts_misc", "draughts_game", "devapp"); // All users can access the misc DB.
        }

        public ISqlTransactionFlavor BeginUserTransaction() => _userDb.Connection().BeginTransaction();
        public ISqlTransactionFlavor BeginAuthUserTransaction() => _authUserDb.Connection().BeginTransaction();
        public ISqlTransactionFlavor BeginGameTransaction() => _gameDb.Connection().BeginTransaction();
        public ISqlTransactionFlavor BeginMiscTransaction() => _miscDb.Connection().BeginTransaction();

        public IInitialQueryBuilder Query(ISqlTransactionFlavor transaction) => QueryBuilder.Init(transaction);

        private record DbConnectionInfo(string Database, string User, string Password) {
            public MySqlFlavor Connection() => new MySqlFlavor(SERVER, PORT, User, Password, Database);
        }
    }
}
