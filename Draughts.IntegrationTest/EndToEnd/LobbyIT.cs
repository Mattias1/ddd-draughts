using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.GameContext.Models;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using static Draughts.Application.Lobby.LobbyController;

namespace Draughts.IntegrationTest.EndToEnd;

public sealed class LobbyIT {
    private readonly ApiTester _apiTester;

    private GameId? GameId;

    public LobbyIT() {
        _apiTester = new ApiTester();
    }

    [Fact]
    public async Task CreateAndJoinGame() {
        await VisitLobbyPageAsGuest();
        await VisitSpectatorPageAsGuest();

        _apiTester.LoginAsTestPlayerBlack();
        await VisitLobbyPage();
        await VisitCreateGamePage();
        await PostCreateGame();

        _apiTester.LoginAsTestPlayerWhite();
        await PostJoinGame();

        AssertGameIsStartedWithCorrectPlayers();
    }

    private async Task VisitLobbyPageAsGuest() {
        string page = await _apiTester.GetString("/lobby");
        page.Should().Contain("<h1>Game lobby</h1>", "This is what guest sees in the the lobby.");
        page.Should().Contain("The list of open games.");
    }

    private async Task VisitSpectatorPageAsGuest() {
        string page = await _apiTester.GetString("/lobby/spectate");
        page.Should().Contain("<h1>Spectator lounge</h1>", "This is what guest sees in the the spectator lounge.");
        page.Should().Contain("Watch one of the games currently being played.");
    }

    private async Task VisitLobbyPage() {
        string page = await _apiTester.GetString("/lobby");
        page.Should().Contain("<h1>Game lobby</h1>", "This is what TestPlayerBlack sees in the the lobby.");
        page.Should().Contain("Join one of the open games");
    }

    private async Task VisitCreateGamePage() {
        string page = await _apiTester.GetString("/lobby/create");
        page.Should().Contain("<h1>Create a game</h1>", "This is what TestPlayerBlack sees in the the create a game page.");
    }

    private async Task PostCreateGame() {
        var result = await _apiTester.PostForm("/lobby/create",
            new GameCreationRequest(6, true, true, true, "max", "black"));
        result.StatusCode.Should().Be(200);
        if (!_apiTester.TryRegex(result.RequestUri(), @"/game/(\d+)", out string? value)) {
            result.RequestUri().Should().Match("/game/<some-value>?success=*");
            return;
        }
        GameId = new GameId(long.Parse(value));
        result.RequestUri().Should().Match($"/game/{GameId}?success=*");
    }

    private async Task PostJoinGame() {
        var result = await _apiTester.PostForm("/lobby/join", new GameJoinRequest(GameId?.Value, null));
        result.StatusCode.Should().Be(200);
        result.RequestUri().Should().Match($"/game/{GameId}?success=*");
    }

    private void AssertGameIsStartedWithCorrectPlayers() {
        _apiTester.UnitOfWork.WithGameTransaction(tran => {
            var createdGame = _apiTester.GameRepository.FindById(GameId!);
            createdGame.StartedAt.Should().NotBeNull();

            createdGame.Players[0].Username.Should().Be(new Username("TestPlayerBlack"));
            createdGame.Players[1].Username.Should().Be(new Username("TestPlayerWhite"));
        });
    }
}
