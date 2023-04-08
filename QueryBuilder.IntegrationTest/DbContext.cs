using AdaskoTheBeAsT.Dapper.NodaTime;
using NodaTime;
using SqlQueryBuilder.Builder;
using SqlQueryBuilder.Options;
using System.Threading.Tasks;

namespace SqlQueryBuilder.IntegrationTest;

public sealed class DbContext {
    private const string SERVER = "localhost";
    private const int PORT = 53306;
    private const string USER = "sqlqb_it_user";
    private const string PASSWORD = "devapp";
    private const string DATABASE = "sqlquirybuilder_it_db";

    private static DbContext? _instance;
    public static DbContext MySql => _instance ??= new DbContext(new MySqlFlavor(SERVER, PORT, USER, PASSWORD, DATABASE));

    private readonly ISqlFlavor _sqlFlavor;

    private DbContext(ISqlFlavor sqlFlavor) {
        _sqlFlavor = sqlFlavor;

        QueryBuilderOptions.SetupDapperWithSnakeCaseAndNodaTime();
    }

    public ISqlTransactionFlavor BeginTransaction() => _sqlFlavor.BeginTransaction();
    public async Task<ISqlTransactionFlavor> BeginTransactionAsync() => await _sqlFlavor.BeginTransactionAsync();

    public IInitialQueryBuilder QueryWithoutTransaction() => QueryBuilder.Init(_sqlFlavor);

    public IInitialQueryBuilder Query(ISqlTransactionFlavor transaction) => QueryBuilder.Init(transaction);
}
