using SqlQueryBuilder.Builder;
using SqlQueryBuilder.Options;

namespace Draughts.Repositories.Database {
    public class DbContext {
        private const int PORT = 52506;
        private static DbContext? _instance = null;
        public static DbContext Get => _instance ??= new DbContext();

        private readonly ISqlFlavor _userDatabase;
        private readonly ISqlFlavor _authUserDatabase;
        private readonly ISqlFlavor _gameDatabase;
        private readonly ISqlFlavor _miscDatabase;

        public DbContext() {
            _userDatabase = BuildMariaDbDatabase("draughts_user", "draughts_user", "devapp");
            _authUserDatabase = BuildMariaDbDatabase("draughts_authuser", "draughts_authuser", "devapp");
            _gameDatabase = BuildMariaDbDatabase("draughts_game", "draughts_game", "devapp");
            _miscDatabase = BuildMariaDbDatabase("draughts_misc", "draughts_game", "devapp"); // All users can access the misc DB.
        }

        private MySqlFlavor BuildMariaDbDatabase(string database, string user, string password) {
            return new MySqlFlavor("localhost", PORT, user, password, database);
        }

        public ISqlTransactionFlavor UserTransaction() => _userDatabase.BeginTransaction();
        public ISqlTransactionFlavor AuthUserTransaction() => _authUserDatabase.BeginTransaction();
        public ISqlTransactionFlavor GameTransaction() => _gameDatabase.BeginTransaction();
        public ISqlTransactionFlavor MiscTransaction() => _miscDatabase.BeginTransaction();

        public IInitialQueryBuilder Query(ISqlTransactionFlavor transaction) => QueryBuilder.Init(transaction);
    }
}
