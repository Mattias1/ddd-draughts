using Draughts.Application.Shared.Middleware;
using Draughts.Common;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories;
using Draughts.Repositories.Database;
using Draughts.Repositories.Databases;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NodaTime;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;

namespace Draughts.IntegrationTest {
    public class ApiTester {
        public static Url BASE_URL = "http://localhost:8080";

        public TestServer Server { get; }
        public FlurlClient Client { get; }
        private string? _cookie;

        // TODO: These repositories don't help yet,
        // as the test talks to a different instance of the database then the application.
        public IClock Clock { get; }
        public IAuthUserRepository AuthUserRepository { get; }
        public IGameRepository GameRepository { get; }
        public IPlayerRepository PlayerRepository { get; }
        public IRoleRepository RoleRepository { get; }
        public IUnitOfWork UnitOfWork { get; }
        public IUserRepository UserRepository { get; }

        public ApiTester() {
            var webHostBuilder = new WebHostBuilder().UseStartup<Startup>();
            Server = new TestServer(webHostBuilder);
            Client = new FlurlClient(Server.CreateClient()).EnableCookies();

            Clock = SystemClock.Instance;
            UnitOfWork = new InMemoryUnitOfWork(Clock);
            RoleRepository = new InMemoryRoleRepository(UnitOfWork);
            AuthUserRepository = new InMemoryAuthUserRepository(RoleRepository, UnitOfWork);
            UserRepository = new InMemoryUserRepository(UnitOfWork);
            PlayerRepository = new InMemoryPlayerRepository(UnitOfWork);
            GameRepository = new InMemoryGameRepository(PlayerRepository, UnitOfWork);
        }

        public void LoginAsTestPlayerBlack() {
            // TODO: Query from database
            var registeredUserRole = new Role(new RoleId(3), Role.REGISTERED_USER_ROLENAME, Permissions.PlayGame);

            var authUserId = new AuthUserId(UserDatabase.TestPlayerBlack);
            var name = new Username("TestPlayerBlack");
            var hash = PasswordHash.Generate("admin", authUserId, name);
            var authUser = new AuthUser(authUserId, new UserId(authUserId), name, hash,
                new Email($"{name}@example.com"), new[] { registeredUserRole });

            LoginAs(authUser);
        }
        public void LoginAsTestPlayerWhite() {
            // TODO: Query from database
            var registeredUserRole = new Role(new RoleId(3), Role.REGISTERED_USER_ROLENAME, Permissions.PlayGame);

            var authUserId = new AuthUserId(UserDatabase.TestPlayerWhite);
            var name = new Username("TestPlayerWhite");
            var hash = PasswordHash.Generate("admin", authUserId, name);
            var authUser = new AuthUser(authUserId, new UserId(authUserId), name, hash,
                new Email($"{name}@example.com"), new[] { registeredUserRole });

            LoginAs(authUser);
        }
        public void LoginAs(AuthUser authUser) {
            _cookie = "Bearer%20" + JsonWebToken.Generate(authUser, Clock).ToJwtString();
        }

        public void Logout() => _cookie = null;

        public async Task<string> GetString(Url url) => await RequestBuilder(url).GetStringAsync();

        public async Task<HttpResponseMessage> PostJson<T>(Url url, T body) => await RequestBuilder(url).PostJsonAsync(body);
        public async Task<HttpResponseMessage> PostForm<T>(Url url, T body) => await RequestBuilder(url).PostUrlEncodedAsync(body);

        private IFlurlRequest RequestBuilder(Url url) {
            var requestBuilder = Client.Request(BASE_URL, url).AllowAnyHttpStatus();
            if (_cookie != null) {
                var expires = Clock.UtcNow().PlusSeconds(JsonWebToken.EXPIRATION_SECONDS).ToDateTimeUtc();
                requestBuilder.WithCookie(AuthContext.AUTHORIZATION_HEADER, _cookie, expires);
            }
            return requestBuilder;
        }

        public bool TryRegex(string haystack, string pattern, [NotNullWhen(returnValue: true)] out string? value, int groupNr = 1) {
            var matches = new Regex(pattern).Matches(haystack, 0);
            if (matches.Count == 0) {
                value = null;
                return false;
            }
            var groups = matches[0].Groups;

            if (groups.Count <= groupNr) {
                value = null;
                return false;
            }
            value = groups[groupNr].Value;
            return true;
        }
    }
}
