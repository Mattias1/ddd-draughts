using FluentAssertions;
using System.Threading.Tasks;
using static Draughts.Application.Auth.AuthController;

namespace Draughts.IntegrationTest.EndToEnd.Base {
    public class AuthenticationApiTester<T> where T : IApiTester {
        public T ApiTester { get; private set; }

        public AuthenticationApiTester(T apiTester) {
            ApiTester = apiTester;
        }

        public async Task VisitRegisterPage() {
            string page = await ApiTester.GetString("/auth/register");
            page.Should().Contain("<h1>Register</h1>");
        }

        public async Task PostRegistration() {
            const string USERNAME = "TestUserRegistration";
            var result = await ApiTester.PostForm("/auth/register", new RegistrationRequest(
                USERNAME, $"{USERNAME}@example.com", "Test password; not secure", "Test password; not secure"
            ));
            result.StatusCode.Should().Be(302);
            result.Headers.Location.Should().Be("/");
        }

        public async Task VisitLoginPage() {
            string page = await ApiTester.GetString("/auth/login");
            page.Should().Contain("<h1>Login</h1>");
        }

        public async Task PostLogin() {
            var result = await ApiTester.PostForm("/auth/login", new LoginRequest("TestPlayerBlack", "admin"));
            result.StatusCode.Should().Be(302);
            result.Headers.Location.Should().Be("/");
        }
    }
}
