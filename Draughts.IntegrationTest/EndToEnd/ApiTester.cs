using DalSoft.Hosting.BackgroundQueue;
using Draughts.Application.Shared;
using Draughts.Application.Shared.Middleware;
using Draughts.Common;
using Draughts.Common.Events;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Models;
using Draughts.Repositories;
using Draughts.Repositories.Misc;
using Draughts.Repositories.Transaction;
using Draughts.Test.Fakes;
using Draughts.Test.TestHelpers;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using NodaTime;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Draughts.IntegrationTest.EndToEnd;

public sealed class ApiTester {
    public static Url BASE_URL = "http://localhost:8080";

    private readonly TestServer Server;
    private readonly FlurlClient Client;
    private string? Cookie;

    public IClock Clock { get; }
    public IIdGenerator IdGenerator { get; }
    public IUnitOfWork UnitOfWork { get; }
    public RoleRepository RoleRepository { get; }
    public AuthUserRepository AuthUserRepository { get; }
    public AdminLogRepository AdminLogRepository { get; }
    public UserRepository UserRepository { get; }
    public GameRepository GameRepository { get; }

    public ApiTester() {
        Server = new TestServer(WebHostBuilder());
        Client = new FlurlClient(Server.CreateClient());

        Clock = SystemClock.Instance;
        IdGenerator = HiLoIdGenerator.BuildHiloGIdGenerator(1, 1, 1);
        IdTestHelper.IdGenerator = IdGenerator;

        // TODO: At this point, maybe just call the service provider and then fetch from the injector???
        var backgroundQueue = new BackgroundQueue(e => Log.Logger.Error("Error in background task", e), 100, 0);
        var eventDispatcher = new EventDispatcher(backgroundQueue, new FakeLogger<EventDispatcher>());
        var unitOfWork = new UnitOfWork(Clock, eventDispatcher, IdGenerator);
        UnitOfWork = unitOfWork;

        RoleRepository = new RoleRepository(unitOfWork);
        AuthUserRepository = new AuthUserRepository(RoleRepository, unitOfWork);
        AdminLogRepository = new AdminLogRepository(unitOfWork);
        UserRepository = new UserRepository(unitOfWork);
        GameRepository = new GameRepository(unitOfWork);
    }

    private IWebHostBuilder WebHostBuilder() {
        var config = Program.LoadAppSettingsConfiguration();
        var appSettings = config.Get<AppSettings?>();

        return new WebHostBuilder()
            .UseConfiguration(config)
            .ConfigureLogging(log => Program.ConfigureSerilog(appSettings, log, "draughts-it"))
            .UseStartup<Startup>();
    }

    public string LoginAsAdmin() => LoginAs("Admin");
    public string LoginAsTestPlayerBlack() => LoginAs("TestPlayerBlack");
    public string LoginAsTestPlayerWhite() => LoginAs("TestPlayerWhite");
    private string LoginAs(string username) {
        var authUser = UnitOfWork.WithAuthTransaction(tran => {
            return AuthUserRepository.FindByName(username);
        });
        return LoginAs(authUser);
    }
    public string LoginAs(AuthUser authUser) {
        return Cookie = "Bearer%20" + JsonWebToken.Generate(authUser, Clock).ToJwtString();
    }

    public ApiTester As(string cookie) {
        Cookie = cookie;
        return this;
    }

    public void Logout() => Cookie = null;

    public async Task<string> GetString(Url url) => await RequestBuilder(url).GetStringAsync();
    public async Task<T?> GetJson<T>(Url url) => await RequestBuilder(url).GetJsonAsync<T>();

    public async Task<IFlurlResponse> Post(Url url) => await RequestBuilder(url).PostAsync();
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

    public async Task WaitForEventsToComplete() => await Task.Delay(1000);
}

public static class HttpResponseMessageExtensions {
    public static string? RequestUri(this IFlurlResponse result)
        => result.ResponseMessage.RequestMessage?.RequestUri?.PathAndQuery;
}
