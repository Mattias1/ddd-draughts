using Draughts.IntegrationTest.EndToEnd.Base;
using System.Threading.Tasks;
using Xunit;

namespace Draughts.IntegrationTest.EndToEnd.InMemory;

[Collection("LobbyIT")]
public class InMemoryLobbyIT {
    private readonly InMemoryApiTester _apiTester;
    private readonly LobbyTesterApi<InMemoryApiTester> _lobbyApi;

    public InMemoryLobbyIT() {
        _apiTester = new InMemoryApiTester();
        _lobbyApi = new LobbyTesterApi<InMemoryApiTester>(_apiTester);
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

        _lobbyApi.AssertGameIsStartedWithCorrectPlayers();
    }
}
