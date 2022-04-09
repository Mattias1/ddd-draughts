using Draughts.IntegrationTest.EndToEnd.Base;
using System.Threading.Tasks;
using Xunit;

namespace Draughts.IntegrationTest.EndToEnd.Database;

[Collection("AuthenticationIT")]
public sealed class DbAuthenticationIT {
    private readonly DbApiTester _apiTest;
    private readonly AuthenticationTesterApi<DbApiTester> _authenticationApi;

    public DbAuthenticationIT() {
        _apiTest = new DbApiTester();
        _authenticationApi = new AuthenticationTesterApi<DbApiTester>(_apiTest);
    }

    [Fact]
    public async Task DontRegisterNewUserWithDuplicateUsername() {
        await _authenticationApi.PostDuplicateUsernameRegistration();
    }

    [Fact]
    public async Task DontRegisterNewUserWithDuplicateEmail() {
        await _authenticationApi.PostDuplicateEmailRegistration();
    }

    [Fact]
    public async Task RegisterNewUser() {
        await _authenticationApi.VisitRegisterPage();
        await _authenticationApi.PostRegistration();

        _authenticationApi.AssertUserIsCreated();
    }

    [Fact]
    public async Task Login() {
        await _authenticationApi.VisitLoginPage();
        await _authenticationApi.PostLogin();
    }
}
