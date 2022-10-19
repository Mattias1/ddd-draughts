using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.AuthContext.Specifications;
using Draughts.Domain.UserContext.Specifications;
using FluentAssertions;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;
using static Draughts.Application.Auth.AuthController;

namespace Draughts.IntegrationTest.EndToEnd;

public sealed class AuthenticationIT {
    private readonly ApiTester _apiTester;

    private Username? CreatedUsername;

    public AuthenticationIT() {
        _apiTester = new ApiTester();
    }

    [Fact]
    public async Task DontRegisterNewUserWithDuplicateUsername() {
        await PostRegistration("MATTY", $"matty+unique@example.com", 400, "/auth/register?error=*");
    }

    [Fact]
    public async Task DontRegisterNewUserWithDuplicateEmail() {
        await PostRegistration("matty_unique", $"MATTY@example.com", 400, "/auth/register?error=*");
    }

    [Fact]
    public async Task RegisterNewUser() {
        await VisitRegisterPage();
        await PostRegistration();

        await AssertUserIsCreated();
    }

    [Fact]
    public async Task Login() {
        await VisitLoginPage();
        await PostLogin();
    }

    private async Task VisitRegisterPage() {
        string page = await _apiTester.GetString("/auth/register");
        page.Should().Contain("<h1>Register</h1>");
    }

    private async Task PostRegistration() {
        string dateTimeString = _apiTester.Clock.UtcNow().ToString("yyyyMMdd-hhmmss", CultureInfo.InvariantCulture);
        string username = $"TestUserR-{dateTimeString}";
        await PostRegistration(username, $"{username}@example.com", 200, "/?success=*");

        CreatedUsername = new Username(username);
    }

    private async Task PostRegistration(string username, string email,
            int expectedStatusCode, string expectedRedirectLocation) {
        var result = await _apiTester.PostForm("/auth/register", new RegistrationRequest(
            username, email, "Test password; not secure", "Test password; not secure"
        ));
        result.StatusCode.Should().Be(expectedStatusCode);
        result.RequestUri().Should().Match(expectedRedirectLocation);
    }

    private async Task VisitLoginPage() {
        string page = await _apiTester.GetString("/auth/login");
        page.Should().Contain("<h1>Login</h1>");
    }

    private async Task PostLogin() {
        var result = await _apiTester.PostForm("/auth/login", new LoginRequest("TestPlayerBlack", "admin"));
        result.StatusCode.Should().Be(200);
        result.RequestUri().Should().Be("/");
    }

    private async Task AssertUserIsCreated() {
        await _apiTester.WaitForEventsToComplete();

        _apiTester.UnitOfWork.WithAuthTransaction(tran => {
            _apiTester.AuthUserRepository.Count(new UsernameSpecification(CreatedUsername)).Should().Be(1);
        });
        _apiTester.UnitOfWork.WithUserTransaction(tran => {
            _apiTester.UserRepository.Count(new UserUsernameSpecification(CreatedUsername)).Should().Be(1);
        });
    }
}
