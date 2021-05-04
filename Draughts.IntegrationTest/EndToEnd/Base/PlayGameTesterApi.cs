using Draughts.Domain.GameContext.Models;
using FluentAssertions;
using System.Threading.Tasks;
using static Draughts.Application.Lobby.LobbyController;
using static Draughts.Application.PlayGame.PlayGameController;

namespace Draughts.IntegrationTest.EndToEnd.Base {
    public class PlayGameTesterApi<T> where T : BaseApiTester {
        public T ApiTester { get; }
        public GameId? GameId { get; private set; }

        public PlayGameTesterApi(T apiTester) {
            ApiTester = apiTester;
        }

        public async Task PostCreateGame(string cookie) {
            var result = await ApiTester.As(cookie).PostForm("/lobby/create",
                new GameCreationRequest(6, true, true, true, "max", "black"));
            result.StatusCode.Should().Be(302);
            if (!ApiTester.TryRegex(result.RedirectLocation(), @"/game/(\d+)", out string? value)) {
                result.RedirectLocation().Should().Match("/game/<some-value>?success=*");
                return;
            }
            GameId = new GameId(long.Parse(value));
            result.RedirectLocation().Should().Match($"/game/{GameId}?success=*");
        }

        public async Task PostJoinGame(string cookie) {
            var result = await ApiTester.As(cookie).PostForm("/lobby/join", new GameJoinRequest(GameId!, null));
            result.StatusCode.Should().Be(302);
            result.RedirectLocation().Should().Match($"/game/{GameId}?success=*");
        }

        public async Task ViewGamePageWithTurn(string turn) {
            string page = await ViewGamePage();
            page.Should().Match($"*Turn: *{turn}*");
        }
        public async Task ViewGamePageWithVictor(string victor) {
            string page = await ViewGamePage();
            page.Should().Match($"*Victor: *{victor}*");
        }
        public async Task<string> ViewGamePage() {
            string page = await ApiTester.GetString($"/game/{GameId}");
            page.Should().Contain($"<h1>Game {GameId}</h1>");
            return page;
        }

        public async Task PostMove(int from, int to, string cookie) {
            var result = await ApiTester.As(cookie).PostJson($"/game/{GameId}/move", new MoveRequest(from, to));
            result.StatusCode.Should().Be(200);
        }
    }
}
