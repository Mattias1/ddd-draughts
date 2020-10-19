using Draughts.Domain.GameAggregate.Models;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using static Draughts.Application.Lobby.LobbyController;

namespace Draughts.IntegrationTest {
    public class LobbyIT {
        private readonly ApiTester _apiTest;
        private GameId? _gameId;

        public LobbyIT() {
            _apiTest = new ApiTester();
        }

        [Fact]
        public async Task CreateAndJoinGame() {
            await VisitLobbyPageAsGuest();

            _apiTest.LoginAsTestPlayerBlack();
            await VisitLobbyPage();
            await VisitCreateGamePage();
            await PostCreateGame();

            _apiTest.LoginAsTestPlayerWhite();
            await PostJoinGame();
        }

        private async Task VisitLobbyPageAsGuest() {
            string page = await _apiTest.GetString("/lobby");
            page.Should().Contain("<h1>Game lobby</h1>", "This is what guest sees in the the lobby.");
            page.Should().Contain("The list of open games");
        }

        private async Task VisitLobbyPage() {
            string page = await _apiTest.GetString("/lobby");
            page.Should().Contain("<h1>Game lobby</h1>", "This is what TestPlayerBlack sees in the the lobby.");
            page.Should().Contain("Join one of the open games");
        }

        private async Task VisitCreateGamePage() {
            string page = await _apiTest.GetString("/lobby/create");
            page.Should().Contain("<h1>Create a game</h1>", "This is what TestPlayerBlack sees in the the create a game page.");
        }

        private async Task PostCreateGame() {
            var result = await _apiTest.PostForm("/lobby/create", new GameCreationRequest {
                BoardSize = 6,
                WhiteHasFirstMove = true,
                FlyingKings = true,
                MenCaptureBackwards = true,
                CaptureConstraints = "max",
                JoinAs = "black",
            });
            result.StatusCode.Should().Be(302);
            if (!_apiTest.TryRegex(result.Headers.Location.ToString(), @"/game/(\d+)", out string? value)) {
                result.Headers.Location.Should().Be("/game/<some-value>");
                return;
            }
            _gameId = new GameId(long.Parse(value));
            result.Headers.Location.Should().Be($"/game/{_gameId}");
        }

        private async Task PostJoinGame() {
            var result = await _apiTest.PostForm("/lobby/join", new GameJoinRequest {
                GameId = _gameId!
            });
            result.StatusCode.Should().Be(302);
            result.Headers.Location.Should().Be($"/game/{_gameId}");
        }
    }
}
