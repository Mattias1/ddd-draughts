using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.AuthContext.Specifications;
using FluentAssertions;
using NodaTime;
using System.Globalization;
using System.Threading.Tasks;
using static Draughts.Application.Auth.AuthController;

namespace Draughts.IntegrationTest.EndToEnd.Base;

public sealed class AuthenticationTesterApi<T> where T : BaseApiTester {
    private readonly IClock _clock;
    public T ApiTester { get; }
    public Username? CreatedUsername { get; private set; }

    public AuthenticationTesterApi(T apiTester) {
        _clock = SystemClock.Instance;
        ApiTester = apiTester;
    }

    public async Task VisitRegisterPage() {
        string page = await ApiTester.GetString("/auth/register");
        page.Should().Contain("<h1>Register</h1>");
    }

    public async Task PostDuplicateUsernameRegistration() {
        await PostRegistration("MATTY", $"matty+unique@example.com", 400, "/auth/register?error=*");
    }

    public async Task PostDuplicateEmailRegistration() {
        await PostRegistration("matty_unique", $"MATTY@example.com", 400, "/auth/register?error=*");
    }

    public async Task PostRegistration() {
        string dateTimeString = _clock.UtcNow().ToString("yyyyMMdd-hhmmss", CultureInfo.InvariantCulture);
        string username = $"TestUserR-{dateTimeString}";
        await PostRegistration(username, $"{username}@example.com", 200, "/?success=*");

        CreatedUsername = new Username(username);
    }

    private async Task PostRegistration(string username, string email,
            int expectedStatusCode, string expectedRedirectLocation) {
        var result = await ApiTester.PostForm("/auth/register", new RegistrationRequest(
            username, email, "Test password; not secure", "Test password; not secure"
        ));
        result.StatusCode.Should().Be(expectedStatusCode);
        result.RequestUri().Should().Match(expectedRedirectLocation);
    }

    public async Task VisitLoginPage() {
        string page = await ApiTester.GetString("/auth/login");
        page.Should().Contain("<h1>Login</h1>");
    }

    public async Task PostLogin() {
        var result = await ApiTester.PostForm("/auth/login", new LoginRequest("TestPlayerBlack", "admin"));
        result.StatusCode.Should().Be(200);
        result.RequestUri().Should().Be("/");
    }

    public void AssertUserIsCreated() {
        ApiTester.UnitOfWork.WithAuthTransaction(tran => {
            ApiTester.AuthUserRepository.Count(new UsernameSpecification(CreatedUsername)).Should().Be(1);
        });
    }
}
