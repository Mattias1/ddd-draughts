using Draughts.IntegrationTest.EndToEnd.Base;
using System.Threading.Tasks;
using Xunit;

namespace Draughts.IntegrationTest.EndToEnd.InMemory {
    public class InMemoryAuthenticationIT {
        private readonly InMemoryApiTester _apiTester;
        private readonly AuthenticationTesterApi<InMemoryApiTester> _authenticationApi;

        public InMemoryAuthenticationIT() {
            _apiTester = new InMemoryApiTester();
            _authenticationApi = new AuthenticationTesterApi<InMemoryApiTester>(_apiTester);
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
}
