using Draughts.Domain.GameAggregate.Models;
using FluentAssertions;
using System.Threading.Tasks;
using static Draughts.Application.Lobby.LobbyController;
using static Draughts.Application.PlayGame.PlayGameController;

namespace Draughts.IntegrationTest.EndToEnd.Base {
    public class PlayGameApiTester<T> where T : IApiTester {
        public T ApiTester { get; }
        public GameId? GameId { get; private set; }

        public PlayGameApiTester(T apiTester) {
            ApiTester = apiTester;
        }

        public async Task PostCreateGame(string cookie) {
            var result = await ApiTester.As(cookie).PostForm("/lobby/create",
                new GameCreationRequest(6, true, true, true, "max", "black"));
            result.StatusCode.Should().Be(302);
            if (!ApiTester.TryRegex(result.Headers.Location?.ToString(), @"/game/(\d+)", out string? value)) {
                result.Headers.Location.Should().Be("/game/<some-value>");
                return;
            }
            GameId = new GameId(long.Parse(value));
            result.Headers.Location.Should().Be($"/game/{GameId}");
        }

        public async Task PostJoinGame(string cookie) {
            var result = await ApiTester.As(cookie).PostForm("/lobby/join", new GameJoinRequest(GameId!, null));
            result.StatusCode.Should().Be(302);
            result.Headers.Location.Should().Be($"/game/{GameId}");
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
