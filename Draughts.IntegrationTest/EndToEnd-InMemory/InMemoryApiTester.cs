using Draughts.Application.Shared.Middleware;
using Draughts.Common;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories.InMemory;
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

namespace Draughts.IntegrationTest.EndToEnd.InMemory {
    public class InMemoryApiTester {
        public static Url BASE_URL = "http://localhost:8080";

        public TestServer Server { get; }
        public FlurlClient Client { get; }
        private string? _cookie;

        public IClock Clock { get; }

        public InMemoryApiTester() {
            var webHostBuilder = new WebHostBuilder().UseStartup<InMemoryStartup>();
            Server = new TestServer(webHostBuilder);
            Client = new FlurlClient(Server.CreateClient()).EnableCookies();

            Clock = SystemClock.Instance;
        }

        public string LoginAsTestPlayerBlack() {
            var registeredUserRole = new Role(new RoleId(3), Role.REGISTERED_USER_ROLENAME, Clock.UtcNow(), Permissions.PlayGame);

            var userId = new UserId(UserDatabase.TestPlayerBlack);
            var name = new Username("TestPlayerBlack");
            var hash = PasswordHash.Generate("admin", userId, name);
            var authUser = new AuthUser(userId, name, hash,
                new Email($"{name}@example.com"), Clock.UtcNow(), new[] { registeredUserRole });

            return LoginAs(authUser);
        }
        public string LoginAsTestPlayerWhite() {
            var registeredUserRole = new Role(new RoleId(3), Role.REGISTERED_USER_ROLENAME, Clock.UtcNow(), Permissions.PlayGame);

            var userId = new UserId(UserDatabase.TestPlayerWhite);
            var name = new Username("TestPlayerWhite");
            var hash = PasswordHash.Generate("admin", userId, name);
            var authUser = new AuthUser(userId, name, hash,
                new Email($"{name}@example.com"), Clock.UtcNow(), new[] { registeredUserRole });

            return LoginAs(authUser);
        }
        public string LoginAs(AuthUser authUser) {
            return _cookie = "Bearer%20" + JsonWebToken.Generate(authUser, Clock).ToJwtString();
        }

        public InMemoryApiTester As(string cookie) {
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
