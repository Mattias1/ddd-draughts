using Draughts.Common.Utilities;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.IntegrationTest.EndToEnd.Base;
using Draughts.Repositories.InMemory;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;

namespace Draughts.IntegrationTest.EndToEnd.InMemory {
    public class InMemoryApiTester : BaseApiTester {
        protected override IWebHostBuilder WebHostBuilder() {
            return new WebHostBuilder()
                .ConfigureAppConfiguration(config => Program.ConfigureAppsettings(config))
                .ConfigureLogging((ctx, log) => Program.ConfigureSerilog(ctx.Configuration, log, "draughts-inmemory-it"))
                .UseStartup<InMemoryStartup>();
        }

        public override string LoginAsAdmin() {
            var adminRole = new Role(new RoleId(1), Role.ADMIN_ROLENAME, Clock.UtcNow(), Permissions.All.ToArray());
            var registeredUserRole = BuildRegisteredUserRole();
            var authUser = BuildTestAuthUser(UserDatabase.AdminId, "Admin", adminRole, registeredUserRole);
            return LoginAs(authUser);
        }
        public override string LoginAsTestPlayerBlack() {
            var registeredUserRole = BuildRegisteredUserRole();
            var authUser = BuildTestAuthUser(UserDatabase.TestPlayerBlack, "TestPlayerBlack", registeredUserRole);
            return LoginAs(authUser);
        }
        public override string LoginAsTestPlayerWhite() {
            Role registeredUserRole = BuildRegisteredUserRole();
            var authUser = BuildTestAuthUser(UserDatabase.TestPlayerWhite, "TestPlayerWhite", registeredUserRole);
            return LoginAs(authUser);
        }

        private Role BuildRegisteredUserRole() {
            return new Role(new RoleId(3), Role.REGISTERED_USER_ROLENAME, Clock.UtcNow(), Permissions.PlayGame);
        }

        private AuthUser BuildTestAuthUser(long id, string name, params Role[] roles) {
            var userId = new UserId(id);
            var username = new Username(name);
            var hash = PasswordHash.Generate("admin", userId, username);
            var email = new Email($"{username}@example.com");
            return new AuthUser(userId, username, hash, email, Clock.UtcNow(), roles);
        }
    }
}
