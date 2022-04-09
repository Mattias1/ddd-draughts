using Draughts.Application.Shared.Middleware;
using Draughts.Common;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NodaTime;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Draughts.IntegrationTest.EndToEnd.Base;

public abstract class BaseApiTester {
    public static Url BASE_URL = "http://localhost:8080";

    public TestServer Server { get; }
    public FlurlClient Client { get; }
    protected string? Cookie { get; private set; }

    public IClock Clock { get; }
    public abstract IIdGenerator IdGenerator { get; }
    public abstract IUnitOfWork UnitOfWork { get; }
    public abstract IRoleRepository RoleRepository { get; }
    public abstract IAuthUserRepository AuthUserRepository { get; }
    public abstract IUserRepository UserRepository { get; }
    public abstract IGameRepository GameRepository { get; }

    protected BaseApiTester() {
        Server = new TestServer(WebHostBuilder());
        Client = new FlurlClient(Server.CreateClient());

        Clock = SystemClock.Instance;
    }

    protected abstract IWebHostBuilder WebHostBuilder();

    public abstract string LoginAsAdmin();
    public abstract string LoginAsTestPlayerBlack();
    public abstract string LoginAsTestPlayerWhite();
    public string LoginAs(AuthUser authUser) {
        return Cookie = "Bearer%20" + JsonWebToken.Generate(authUser, Clock).ToJwtString();
    }

    public BaseApiTester As(string cookie) {
        Cookie = cookie;
        return this;
    }

    public void Logout() => Cookie = null;

    public async Task<string> GetString(Url url) => await RequestBuilder(url).GetStringAsync();
    public async Task<T?> GetJson<T>(Url url) => await RequestBuilder(url).GetJsonAsync<T>();

    public async Task<IFlurlResponse> Post(Url url) => await RequestBuilder(url).PostAsync(null);
    public async Task<IFlurlResponse> PostJson<T>(Url url, T body) => await RequestBuilder(url).PostJsonAsync(body);
    public async Task<IFlurlResponse> PostForm<T>(Url url, T body) => await RequestBuilder(url).PostUrlEncodedAsync(body);

    private IFlurlRequest RequestBuilder(Url url) {
        var requestBuilder = Client.Request(BASE_URL, url).AllowAnyHttpStatus();
        if (Cookie is not null) {
            var expires = Clock.UtcNow().PlusSeconds(JsonWebToken.EXPIRATION_SECONDS).ToDateTimeUtc();
            var jar = new CookieJar().AddOrReplace(AuthContext.AUTHORIZATION_HEADER, Cookie, Client.BaseUrl, expires);
            requestBuilder.WithCookies(jar);
        }
        return requestBuilder;
    }

    public bool TryRegex(string? haystack, string pattern,
            [NotNullWhen(returnValue: true)] out string? value, int groupNr = 1) {
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

public static class HttpResponseMessageExtensions {
    public static string? RequestUri(this IFlurlResponse result)
        => result.ResponseMessage.RequestMessage?.RequestUri?.PathAndQuery;
}
