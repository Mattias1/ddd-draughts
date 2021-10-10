using Draughts.IntegrationTest.EndToEnd.Base;
using Draughts.Repositories;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using Microsoft.AspNetCore.Hosting;

namespace Draughts.IntegrationTest.EndToEnd.Database {
    public class DbApiTester : BaseApiTester {
        public override IIdGenerator IdGenerator { get; }
        public override IUnitOfWork UnitOfWork { get; }
        public override IRoleRepository RoleRepository { get; }
        public override IAuthUserRepository AuthUserRepository { get; }
        public override IUserRepository UserRepository { get; }
        public override IGameRepository GameRepository { get; }

        public DbApiTester() {
            IdGenerator = HiLoIdGenerator.DbHiloGIdGenerator(1, 1, 1);
            UnitOfWork = new DbUnitOfWork(Clock, IdGenerator);
            RoleRepository = new DbRoleRepository(UnitOfWork);
            AuthUserRepository = new DbAuthUserRepository(RoleRepository, UnitOfWork);
            UserRepository = new DbUserRepository(UnitOfWork);
            GameRepository = new DbGameRepository(UnitOfWork);
        }

        protected override IWebHostBuilder WebHostBuilder() {
            return new WebHostBuilder()
                .ConfigureAppConfiguration(config => Program.ConfigureAppsettings(config))
                .ConfigureLogging((ctx, log) => Program.ConfigureSerilog(ctx.Configuration, log, "draughts-db-it"))
                .UseStartup<DbStartup>();
        }

        public override string LoginAsAdmin() => LoginAs("Admin");
        public override string LoginAsTestPlayerBlack() => LoginAs("TestPlayerBlack");
        public override string LoginAsTestPlayerWhite() => LoginAs("TestPlayerWhite");
        private string LoginAs(string username) {
            var authUser = UnitOfWork.WithAuthTransaction(tran => {
                return AuthUserRepository.FindByName(username);
            });
            return LoginAs(authUser);
        }
    }
}
