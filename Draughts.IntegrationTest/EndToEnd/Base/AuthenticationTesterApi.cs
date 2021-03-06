using Draughts.Common.Utilities;
using Draughts.Domain.AuthUserContext.Models;
using FluentAssertions;
using NodaTime;
using System.Globalization;
using System.Threading.Tasks;
using static Draughts.Application.Auth.AuthController;

namespace Draughts.IntegrationTest.EndToEnd.Base {
    public class AuthenticationTesterApi<T> where T : BaseApiTester {
        private readonly IClock _clock;
        public T ApiTester { get; private set; }

        public AuthenticationTesterApi(T apiTester) {
            _clock = SystemClock.Instance;
            ApiTester = apiTester;
        }

        public async Task VisitRegisterPage() {
            string page = await ApiTester.GetString("/auth/register");
            page.Should().Contain("<h1>Register</h1>");
        }

        public async Task PostDuplicateUsernameRegistration() {
            await PostRegistration("MATTY", $"matty+unique@example.com", "/auth/register?error=*");
        }

        public async Task PostDuplicateEmailRegistration() {
            await PostRegistration("matty_unique", $"MATTY@example.com", "/auth/register?error=*");
        }

        public async Task PostRegistration() {
            string dateTimeString = _clock.UtcNow().ToString("yyyyMMdd-hhmmss", CultureInfo.InvariantCulture);
            string username = $"TestUserR-{dateTimeString}";
            await PostRegistration(username, $"{username}@example.com", "/?success=*");
        }

        private async Task PostRegistration(string username, string email, string expectedRedirectLocation) {
            var result = await ApiTester.PostForm("/auth/register", new RegistrationRequest(
                username, email, "Test password; not secure", "Test password; not secure"
            ));
            result.StatusCode.Should().Be(302);
            result.RedirectLocation().Should().Match(expectedRedirectLocation);
        }

        public async Task VisitLoginPage() {
            string page = await ApiTester.GetString("/auth/login");
            page.Should().Contain("<h1>Login</h1>");
        }

        public async Task PostLogin() {
            var result = await ApiTester.PostForm("/auth/login", new LoginRequest("TestPlayerBlack", "admin"));
            result.StatusCode.Should().Be(302);
            result.RedirectLocation().Should().Be("/");
        }
    }
}
