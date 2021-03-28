using Draughts.Domain.GameAggregate.Models;
using Draughts.IntegrationTest.EndToEnd.Base;
using System.Threading.Tasks;
using Xunit;

namespace Draughts.IntegrationTest.EndToEnd.Database {
    public class DbLobbyIT {
        private readonly DbApiTester _apiTester;
        private readonly LobbyTesterApi<DbApiTester> _lobbyApi;

        public DbLobbyIT() {
            _apiTester = new DbApiTester();
            _lobbyApi = new LobbyTesterApi<DbApiTester>(_apiTester);
        }

        [Fact]
        public async Task CreateAndJoinGame() {
            await _lobbyApi.VisitLobbyPageAsGuest();
            await _lobbyApi.VisitSpectatorPageAsGuest();

            _apiTester.LoginAsTestPlayerBlack();
            await _lobbyApi.VisitLobbyPage();
            await _lobbyApi.VisitCreateGamePage();
            await _lobbyApi.PostCreateGame();

            _apiTester.LoginAsTestPlayerWhite();
            await _lobbyApi.PostJoinGame();
        }
    }
}
