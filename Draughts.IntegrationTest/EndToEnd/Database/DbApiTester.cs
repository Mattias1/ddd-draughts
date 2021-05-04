using Draughts.Domain.AuthUserContext.Specifications;
using Draughts.IntegrationTest.EndToEnd.Base;
using Draughts.Repositories;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using Microsoft.AspNetCore.Hosting;

namespace Draughts.IntegrationTest.EndToEnd.Database {
    public class DbApiTester : BaseApiTester {
        public IIdGenerator IdGenerator { get; }
        public IUnitOfWork UnitOfWork { get; }
        public IRoleRepository RoleRepository { get; }
        public IAuthUserRepository AuthUserRepository { get; }
        public IUserRepository UserRepository { get; }
        public IGameRepository GameRepository { get; }

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
            var authUser = UnitOfWork.WithAuthUserTransaction(tran => {
                var authUser = AuthUserRepository.Find(new UsernameSpecification(username));
                return tran.CommitWith(authUser);
            });
            return LoginAs(authUser);
        }
    }
}
