using Draughts.Domain.AuthUserAggregate.Models;
using Flurl;
using Flurl.Http;
using Microsoft.AspNetCore.TestHost;
using NodaTime;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace Draughts.IntegrationTest.EndToEnd.Base {
    public interface IApiTester {
        TestServer Server { get; }
        FlurlClient Client { get; }
        IClock Clock { get; }

        IApiTester As(string cookie);
        Task<string> GetString(Url url);
        string LoginAs(AuthUser authUser);
        string LoginAsAdmin();
        string LoginAsTestPlayerBlack();
        string LoginAsTestPlayerWhite();
        void Logout();
        Task<HttpResponseMessage> Post(Url url);
        Task<HttpResponseMessage> PostForm<T>(Url url, T body);
        Task<HttpResponseMessage> PostJson<T>(Url url, T body);
        bool TryRegex(string? haystack, string pattern, [NotNullWhen(true)] out string? value, int groupNr = 1);
    }

    public static class HttpResponseMessageExtensions {
        public static string? RedirectLocation(this HttpResponseMessage result) => result.Headers.Location?.ToString();
    }
}
