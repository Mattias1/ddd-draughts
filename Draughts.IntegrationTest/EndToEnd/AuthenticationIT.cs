using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.AuthContext.Specifications;
using Draughts.Domain.UserContext.Specifications;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Xunit;
using static Draughts.Application.Auth.AuthController;
using static Draughts.Application.Users.UsersController;

namespace Draughts.IntegrationTest.EndToEnd;

public sealed class AuthenticationIT {
    private readonly ApiTester _apiTester;
    private Username? _createdUsername;

    public AuthenticationIT() {
        _apiTester = new ApiTester();
    }

    [Fact]
    public async Task DontRegisterNewUserWithDuplicateUsername() {
        await PostRegistration("MATTY", "matty+unique@example.com", 400, "/auth/register?error=*");
    }

    [Fact]
    public async Task DontRegisterNewUserWithDuplicateEmail() {
        await PostRegistration("matty_unique", "MATTY@example.com", 400, "/auth/register?error=*");
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

    [Fact]
    public async Task EditAccountSettings() {
        CreateUserAndLogin();
        await VisitAccountSettingsPage();
        await PostEditAccountSettings();
    }

    private async Task VisitRegisterPage() {
        string page = await _apiTester.GetString("/auth/register");
        page.Should().Contain("<h1>Register</h1>");
    }

    private async Task PostRegistration() {
        string dateTimeString = _apiTester.Clock.UtcNow().ToString("yyyyMMdd-hhmmss", CultureInfo.InvariantCulture);
        string username = $"TestUserR-{dateTimeString}";
        await PostRegistration(username, $"{username}@example.com", 200, "/?success=*");

        _createdUsername = new Username(username);
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
        result.Should().HaveStatusCode(200);
        result.RequestUri().Should().Be("/");
    }

    private void CreateUserAndLogin() {
        string dateTimeString = _apiTester.Clock.UtcNow().ToString("yyyyMMdd-hhmmss", CultureInfo.InvariantCulture);
        string username = $"TestUserE-{dateTimeString}";

        var user = UserTestHelper.User(username).Build();
        _apiTester.UnitOfWork.WithUserTransaction(tran => {
            _apiTester.UserRepository.Save(user);
        });

        var registeredUserRole = RoleTestHelper.RegisteredUser().WithId(2).Build();
        var authUser = AuthUserTestHelper.FromUserAndRoles(user, registeredUserRole).Build();
        _apiTester.UnitOfWork.WithAuthTransaction(tran => {
            _apiTester.AuthUserRepository.Save(authUser);
        });

        _createdUsername = user.Username;
        _apiTester.LoginAs(authUser);
    }

    private async Task VisitAccountSettingsPage() {
        string page = await _apiTester.GetString("/user/account");
        page.Should().Contain("<h1>Account settings</h1>");
    }

    private async Task PostEditAccountSettings() {
        if (_createdUsername is null) {
            throw new InvalidOperationException("We should have a user here");
        }
        var request = new AccountSettingsRequest("new_email@example.com", "new_password", "new_password",
            _createdUsername.Value);
        var result = await _apiTester.PostForm("/user/account", request);
        result.Should().HaveStatusCode(200);
        result.RequestUri().Should().Match("/user/account?success=*");

        _apiTester.UnitOfWork.WithAuthTransaction(tran => {
            var authUser = _apiTester.AuthUserRepository.FindByName(_createdUsername.Value);
            authUser.Email.Value.Should().Be("new_email@example.com");
            authUser.PasswordHash.CanLogin("new_password", authUser.Id, authUser.Username).Should().BeTrue();
        });
    }

    private async Task AssertUserIsCreated() {
        await _apiTester.WaitForEventsToComplete();

        _apiTester.UnitOfWork.WithAuthTransaction(tran => {
            _apiTester.AuthUserRepository.Count(new UsernameSpecification(_createdUsername)).Should().Be(1);
        });
        _apiTester.UnitOfWork.WithUserTransaction(tran => {
            _apiTester.UserRepository.Count(new UserUsernameSpecification(_createdUsername)).Should().Be(1);
        });
    }
}
