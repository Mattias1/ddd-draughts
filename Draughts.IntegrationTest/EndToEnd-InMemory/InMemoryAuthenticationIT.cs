using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using static Draughts.Application.Auth.AuthController;

namespace Draughts.IntegrationTest.EndToEnd.InMemory {
    public class InMemoryAuthenticationIT {
        private readonly InMemoryApiTester _apiTest;

        public InMemoryAuthenticationIT() {
            _apiTest = new InMemoryApiTester();
        }

        [Fact]
        public async Task RegisterNewUser() {
            await VisitRegisterPage();
            await PostRegistration();
        }

        private async Task VisitRegisterPage() {
            string page = await _apiTest.GetString("/auth/register");
            page.Should().Contain("<h1>Register</h1>");
        }

        private async Task PostRegistration() {
            const string USERNAME = "TestUserRegistration";
            var result = await _apiTest.PostForm("/auth/register", new RegistrationRequest(
                USERNAME, $"{USERNAME}@example.com", "Test password; not secure", "Test password; not secure"
            ));
            result.StatusCode.Should().Be(302);
            result.Headers.Location.Should().Be("/");
        }

        [Fact]
        public async Task Login() {
            await VisitLoginPage();
            await PostLogin();
        }

        private async Task VisitLoginPage() {
            string page = await _apiTest.GetString("/auth/login");
            page.Should().Contain("<h1>Login</h1>");
        }

        private async Task PostLogin() {
            var result = await _apiTest.PostForm("/auth/login", new LoginRequest("TestPlayerBlack", "admin"));
            result.StatusCode.Should().Be(302);
            result.Headers.Location.Should().Be("/");
        }
    }
}
