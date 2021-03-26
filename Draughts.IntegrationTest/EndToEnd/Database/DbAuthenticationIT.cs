using Draughts.IntegrationTest.EndToEnd.Base;
using System.Threading.Tasks;
using Xunit;

namespace Draughts.IntegrationTest.EndToEnd.Database {
    public class DbAuthenticationIT {
        private readonly DbApiTester _apiTest;
        private readonly AuthenticationApiTester<DbApiTester> _authenticationApi;

        public DbAuthenticationIT() {
            _apiTest = new DbApiTester();
            _authenticationApi = new AuthenticationApiTester<DbApiTester>(_apiTest);
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
