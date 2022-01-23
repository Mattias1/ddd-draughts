using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.IntegrationTest.EndToEnd.Base;
using Draughts.Repositories;
using Draughts.Repositories.InMemory;
using Draughts.Repositories.Transaction;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using static Draughts.Domain.AuthContext.Models.Permission;

namespace Draughts.IntegrationTest.EndToEnd.InMemory;

public class InMemoryApiTester : BaseApiTester {
    public override IIdGenerator IdGenerator { get; }
    public override IUnitOfWork UnitOfWork { get; }
    public override IRoleRepository RoleRepository { get; }
    public override IAuthUserRepository AuthUserRepository { get; }
    public override IUserRepository UserRepository { get; }
    public override IGameRepository GameRepository { get; }

    public InMemoryApiTester() {
        IdGenerator = HiLoIdGenerator.InMemoryHiloGIdGenerator(1, 1, 1);
        UnitOfWork = new InMemoryUnitOfWork(Clock, IdGenerator);
        RoleRepository = new InMemoryRoleRepository(UnitOfWork);
        AuthUserRepository = new InMemoryAuthUserRepository(RoleRepository, UnitOfWork);
        UserRepository = new InMemoryUserRepository(UnitOfWork);
        GameRepository = new InMemoryGameRepository(UnitOfWork);
    }

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
        return new AuthUser(userId, username, hash, email, Clock.UtcNow(), roles.Select(r => r.Id));
    }
}
