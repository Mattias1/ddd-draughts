using Draughts.Application.Shared.Middleware;
using Draughts.Common;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.AuthUserAggregate.Specifications;
using Draughts.Repositories;
using Draughts.Repositories.Database;
using Draughts.Repositories.Transaction;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NodaTime;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Draughts.IntegrationTest.EndToEnd.Database {
    public class ApiTester {
        public static Url BASE_URL = "http://localhost:8080";

        public TestServer Server { get; }
        public FlurlClient Client { get; }
        private string? _cookie;

        public IClock Clock { get; }
        public IIdGenerator IdGenerator { get; }
        public IUnitOfWork UnitOfWork { get; }
        public IRoleRepository RoleRepository { get; }
        public IAuthUserRepository AuthUserRepository { get; }
        public IUserRepository UserRepository { get; }
        public IGameRepository GameRepository { get; }
        public IPlayerRepository PlayerRepository { get; }

        public ApiTester() {
            var webHostBuilder = new WebHostBuilder().UseStartup<Startup>();
            Server = new TestServer(webHostBuilder);
            Client = new FlurlClient(Server.CreateClient()).EnableCookies();

            Clock = SystemClock.Instance;
            IdGenerator = HiLoIdGenerator.DbHiloGIdGenerator(1, 1, 1);
            UnitOfWork = new DbUnitOfWork(Clock, IdGenerator);
            RoleRepository = new DbRoleRepository(UnitOfWork);
            AuthUserRepository = new DbAuthUserRepository(RoleRepository, UnitOfWork);
            UserRepository = new DbUserRepository(UnitOfWork);
            GameRepository = new DbGameRepository(UnitOfWork);
            PlayerRepository = new DbPlayerRepository(UnitOfWork);
        }

        public string LoginAsTestPlayerBlack() => LoginAs("TestPlayerBlack");
        public string LoginAsTestPlayerWhite() => LoginAs("TestPlayerWhite");
        public string LoginAs(string username) {
            var authUser = UnitOfWork.WithAuthUserTransaction(tran => {
                var authUser = AuthUserRepository.Find(new UsernameSpecification(username));
                return tran.CommitWith(authUser);
            });
            return LoginAs(authUser);
        }
        public string LoginAs(AuthUser authUser) {
            return _cookie = "Bearer%20" + JsonWebToken.Generate(authUser, Clock).ToJwtString();
        }

        public ApiTester As(string cookie) {
            _cookie = cookie;
            return this;
        }

        public void Logout() => _cookie = null;

        public async Task<string> GetString(Url url) => await RequestBuilder(url).GetStringAsync();

        public async Task<HttpResponseMessage> PostJson<T>(Url url, T body) => await RequestBuilder(url).PostJsonAsync(body);
        public async Task<HttpResponseMessage> PostForm<T>(Url url, T body) => await RequestBuilder(url).PostUrlEncodedAsync(body);

        private IFlurlRequest RequestBuilder(Url url) {
            var requestBuilder = Client.Request(BASE_URL, url).AllowAnyHttpStatus();
            if (_cookie is not null) {
                var expires = Clock.UtcNow().PlusSeconds(JsonWebToken.EXPIRATION_SECONDS).ToDateTimeUtc();
                requestBuilder.WithCookie(AuthContext.AUTHORIZATION_HEADER, _cookie, expires);
            }
            return requestBuilder;
        }

        public bool TryRegex(string? haystack, string pattern, [NotNullWhen(returnValue: true)] out string? value, int groupNr = 1) {
            var matches = new Regex(pattern).Matches(haystack ?? "", 0);
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
