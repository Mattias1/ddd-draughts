using Draughts.Domain.GameAggregate.Models;
using FluentAssertions;
using System.Threading.Tasks;
using static Draughts.Application.Lobby.LobbyController;

namespace Draughts.IntegrationTest.EndToEnd.Base {
    public class LobbyApiTester<T> where T : IApiTester {
        public T ApiTester { get; }
        public GameId? GameId { get; private set; }

        public LobbyApiTester(T apiTester) {
            ApiTester = apiTester;
        }

        public async Task VisitLobbyPageAsGuest() {
            string page = await ApiTester.GetString("/lobby");
            page.Should().Contain("<h1>Game lobby</h1>", "This is what guest sees in the the lobby.");
            page.Should().Contain("The list of open games.");
        }

        public async Task VisitSpectatorPageAsGuest() {
            string page = await ApiTester.GetString("/lobby/spectate");
            page.Should().Contain("<h1>Spectator lounge</h1>", "This is what guest sees in the the spectator lounge.");
            page.Should().Contain("Watch one of the games currently being played.");
        }

        public async Task VisitLobbyPage() {
            string page = await ApiTester.GetString("/lobby");
            page.Should().Contain("<h1>Game lobby</h1>", "This is what TestPlayerBlack sees in the the lobby.");
            page.Should().Contain("Join one of the open games");
        }

        public async Task VisitCreateGamePage() {
            string page = await ApiTester.GetString("/lobby/create");
            page.Should().Contain("<h1>Create a game</h1>", "This is what TestPlayerBlack sees in the the create a game page.");
        }

        public async Task PostCreateGame() {
            var result = await ApiTester.PostForm("/lobby/create",
                new GameCreationRequest(6, true, true, true, "max", "black"));
            result.StatusCode.Should().Be(302);
            if (!ApiTester.TryRegex(result.RedirectLocation(), @"/game/(\d+)", out string? value)) {
                result.RedirectLocation().Should().Match("/game/<some-value>?success=*");
                return;
            }
            GameId = new GameId(long.Parse(value));
            result.RedirectLocation().Should().Match($"/game/{GameId}?success=*");
        }

        public async Task PostJoinGame() {
            var result = await ApiTester.PostForm("/lobby/join", new GameJoinRequest(GameId!, null));
            result.StatusCode.Should().Be(302);
            result.RedirectLocation().Should().Match($"/game/{GameId}?success=*");
        }
    }
}
