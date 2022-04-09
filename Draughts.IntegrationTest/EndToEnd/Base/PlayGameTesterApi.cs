using Draughts.Application.Shared.ViewModels;
using Draughts.Domain.GameContext.Models;
using FluentAssertions;
using System.Linq;
using System.Threading.Tasks;
using static Draughts.Application.Lobby.LobbyController;
using static Draughts.Application.PlayGame.PlayGameController;

namespace Draughts.IntegrationTest.EndToEnd.Base;

public sealed class PlayGameTesterApi<T> where T : BaseApiTester {
    public T ApiTester { get; }
    public GameId? GameId { get; private set; }

    public PlayGameTesterApi(T apiTester) {
        ApiTester = apiTester;
    }

    public async Task PostCreateGame(string cookie) {
        var result = await ApiTester.As(cookie).PostForm("/lobby/create",
            new GameCreationRequest(6, true, true, true, "max", "black"));
        result.StatusCode.Should().Be(200);
        if (!ApiTester.TryRegex(result.RequestUri(), @"/game/(\d+)", out string? value)) {
            result.RequestUri().Should().Match("/game/<some-value>?success=*");
            return;
        }
        GameId = new GameId(long.Parse(value));
        result.RequestUri().Should().Match($"/game/{GameId}?success=*");
    }

    public async Task PostJoinGame(string cookie) {
        var result = await ApiTester.As(cookie).PostForm("/lobby/join", new GameJoinRequest(GameId?.Value, null));
        result.StatusCode.Should().Be(200);
        result.RequestUri().Should().Match($"/game/{GameId}?success=*");
    }

    public async Task ViewGameJsonWithTurn(Color expectedTurn) {
        var json = await ViewGameJson();

        ApiTester.UnitOfWork.WithGameTransaction(tran => {
            var game = ApiTester.GameRepository.FindById(GameId!);

            var playerWithColor = game.Players.SingleOrDefault(p => p.Color == expectedTurn);
            json.Turn?.PlayerId.Should().Be(playerWithColor?.Id.Value);
        });
    }
    public async Task ViewGamePageWithVictor(string victor) {
        string page = await ViewGamePage();
        page.Should().Match($"*Victor: *{victor}*");
    }
    public async Task<string> ViewGamePage() {
        string page = await ApiTester.GetString($"/game/{GameId}");
        page.Should().Contain($"Game {GameId}</h1>");
        return page;
    }
    public async Task<GameDto> ViewGameJson() {
        var json = await ApiTester.GetJson<GameDto>($"/game/{GameId}/json");
        json.Should().NotBeNull();
        return json!;
    }

    public async Task PostMove(int from, int to, string cookie) {
        var result = await ApiTester.As(cookie).PostJson($"/game/{GameId}/move", new MoveRequest(from, to));
        result.StatusCode.Should().Be(200);
    }

    public async Task PostDrawVote(string cookie) {
        var result = await ApiTester.As(cookie).Post($"/game/{GameId}/draw");
        result.StatusCode.Should().Be(200);
        result.RequestUri().Should().Match($"/game/{GameId}?success=*");
    }

    // TODO: What to do here when events are handled in a different thread?
    public void AssertUserStatisticsAreUpdatedCorrectly() {
        ApiTester.UnitOfWork.WithUserTransaction(tran => {
            var blackUser = ApiTester.UserRepository.FindByName("TestPlayerBlack");
            var whiteUser = ApiTester.UserRepository.FindByName("TestPlayerWhite");
            blackUser.Statistics.OtherTally.Won.Should().BeGreaterThan(0);
            whiteUser.Statistics.OtherTally.Lost.Should().BeGreaterThan(0);
        });
    }

    public void AssertGameIsDraw() {
        ApiTester.UnitOfWork.WithGameTransaction(tran => {
            var game = ApiTester.GameRepository.FindById(GameId!);

            game.FinishedAt.Should().NotBeNull();
            game.Victor.Should().BeNull();
            game.Turn.Should().BeNull();
        });
    }
}
