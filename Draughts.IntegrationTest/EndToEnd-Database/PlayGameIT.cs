using Draughts.Domain.GameAggregate.Models;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using static Draughts.Application.Lobby.LobbyController;
using static Draughts.Application.PlayGame.PlayGameController;

namespace Draughts.IntegrationTest.EndToEnd.Database {
    public class PlayGameIT {
        private readonly ApiTester _apiTest;
        private GameId? _gameId;

        public PlayGameIT() {
            _apiTest = new ApiTester();
        }

        [Fact]
        public async Task PlayGame() {
            string black = _apiTest.LoginAsTestPlayerBlack();
            await PostCreateGame(black);

            string white = _apiTest.LoginAsTestPlayerWhite();
            await PostJoinGame(white);

            await ViewGamePageWithTurn("white");

            // |_|4|_|4|_|4|        // |_|1|_|2|_|3|
            // |4|_|4|_|4|_|        // |4|_|5|_|6|_|
            // |_|.|_|.|_|.|        // |_|7|_|8|_|9|
            // |.|_|.|_|.|_|        // 10|_11|_12|_|
            // |_|5|_|5|_|5|        // |_13|_14|_15|
            // |5|_|5|_|5|_|        // 16|_17|_18|_|
            await PostMove(13, 11, white);
            await PostMove(6, 9, black);

            await PostMove(11, 8, white);
            await PostMove(5, 12, black);

            // |_|4|_|4|_|4|
            // |4|_|.|_|.|_|
            // |_|.|_|.|_|4|
            // |.|_|.|_|4|_|
            // |_|.|_|5|_|5|
            // |5|_|5|_|5|_|
            await PostMove(15, 8, white);
            await PostMove(2, 6, black);

            await PostMove(16, 13, white);
            await PostMove(6, 11, black);
            await PostMove(11, 16, black);

            // |_|4|_|.|_|4|
            // |4|_|.|_|.|_|
            // |_|.|_|.|_|4|
            // |.|_|.|_|.|_|
            // |_|.|_|5|_|.|
            // |6|_|5|_|5|_|
            await PostMove(14, 12, white);
            await PostMove(9, 14, black);

            await PostMove(18, 11, white);
            await PostMove(16, 8, black);

            // |_|4|_|.|_|4|
            // |4|_|.|_|.|_|
            // |_|.|_|6|_|.|
            // |.|_|.|_|.|_|
            // |_|.|_|.|_|.|
            // |.|_|5|_|.|_|
            await PostMove(17, 13, white);
            await PostMove(8, 16, black);

            await ViewGamePageWithVictor("TestPlayerBlack");
        }

        private async Task PostCreateGame(string cookie) {
            var result = await _apiTest.As(cookie).PostForm("/lobby/create", new GameCreationRequest(
                6, true, true, true, "max", "black"));
            result.StatusCode.Should().Be(302);
            if (!_apiTest.TryRegex(result.Headers.Location?.ToString(), @"/game/(\d+)", out string? value)) {
                result.Headers.Location.Should().Be("/game/<some-value>");
                return;
            }
            _gameId = new GameId(long.Parse(value));
            result.Headers.Location.Should().Be($"/game/{_gameId}");
        }

        private async Task PostJoinGame(string cookie) {
            var result = await _apiTest.As(cookie).PostForm("/lobby/join", new GameJoinRequest(_gameId!, null));
            result.StatusCode.Should().Be(302);
            result.Headers.Location.Should().Be($"/game/{_gameId}");
        }

        private async Task ViewGamePageWithTurn(string turn) {
            string page = await ViewGamePage();
            page.Should().Match($"*Turn: *{turn}*");
        }
        private async Task ViewGamePageWithVictor(string victor) {
            string page = await ViewGamePage();
            page.Should().Match($"*Victor: *{victor}*");
        }
        private async Task<string> ViewGamePage() {
            string page = await _apiTest.GetString($"/game/{_gameId}");
            page.Should().Contain($"<h1>Game {_gameId}</h1>");
            return page;
        }

        private async Task PostMove(int from, int to, string cookie) {
            var result = await _apiTest.As(cookie).PostJson($"/game/{_gameId}/move", new MoveRequest(from, to));
            result.StatusCode.Should().Be(200);
        }
    }
}
