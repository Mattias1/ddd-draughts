using Draughts.IntegrationTest.EndToEnd.Base;
using System.Threading.Tasks;
using Xunit;

namespace Draughts.IntegrationTest.EndToEnd.InMemory {
    public class InMemoryAuthenticationIT {
        private readonly InMemoryApiTester _apiTester;
        private readonly AuthenticationApiTester<InMemoryApiTester> _authenticationApi;

        public InMemoryAuthenticationIT() {
            _apiTester = new InMemoryApiTester();
            _authenticationApi = new AuthenticationApiTester<InMemoryApiTester>(_apiTester);
        }

        [Fact]
        public async Task RegisterNewUser() {
            await _authenticationApi.VisitRegisterPage();
            await _authenticationApi.PostRegistration();
        }

        [Fact]
        public async Task Login() {
            await _authenticationApi.VisitLoginPage();
            await _authenticationApi.PostLogin();
        }
    }
}
