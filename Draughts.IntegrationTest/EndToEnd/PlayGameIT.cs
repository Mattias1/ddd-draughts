using Draughts.Application.Shared.ViewModels;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.UserContext.Models;
using FluentAssertions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using static Draughts.Application.Lobby.LobbyController;
using static Draughts.Application.PlayGame.PlayGameController;

namespace Draughts.IntegrationTest.EndToEnd;

public sealed class PlayGameIT {
    private readonly ApiTester _apiTester;

    private UserStatistics? _recordedBlackUserStats;
    private UserStatistics? _recordedWhiteUserStats;

    private GameId? GameId;

    public PlayGameIT() {
        _apiTester = new ApiTester();
    }

    [Fact]
    public async Task PlayGame() {
        string black = _apiTester.LoginAsTestPlayerBlack();
        await PostCreateGame(black);

        string white = _apiTester.LoginAsTestPlayerWhite();
        await PostJoinGame(white);

        RecordUserStatistics();

        await ViewGameJsonWithTurn(Color.White);

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
        await AssertUserStatisticsDiff(
            blackStats => blackStats.OtherTally.Won, 1,
            whiteStats => whiteStats.OtherTally.Lost, 1
        );
    }

    [Fact]
    public async Task DrawGame() {
        string black = _apiTester.LoginAsTestPlayerBlack();
        await PostCreateGame(black);

        string white = _apiTester.LoginAsTestPlayerWhite();
        await PostJoinGame(white);

        RecordUserStatistics();

        await PostDrawVote(white);
        await PostDrawVote(black);

        AssertGameIsDraw();
        await AssertUserStatisticsDiff(
            blackStats => blackStats.OtherTally.Tied, 1,
            whiteStats => whiteStats.OtherTally.Tied, 1
        );
    }

    [Fact]
    public async Task ResignGame() {
        string black = _apiTester.LoginAsTestPlayerBlack();
        await PostCreateGame(black);

        string white = _apiTester.LoginAsTestPlayerWhite();
        await PostJoinGame(white);

        RecordUserStatistics();

        await PostResignation(white);

        await ViewGamePageWithVictor("TestPlayerBlack");
        await AssertUserStatisticsDiff(
            blackStats => blackStats.OtherTally.Won, 1,
            whiteStats => whiteStats.OtherTally.Lost, 1
        );
    }

    private async Task PostCreateGame(string cookie) {
        var result = await _apiTester.As(cookie).PostForm("/lobby/create",
            new GameCreationRequest(6, true, true, true, "max", "black"));
        result.StatusCode.Should().Be(200);
        if (!_apiTester.TryRegex(result.RequestUri(), @"/game/(\d+)", out string? value)) {
            result.RequestUri().Should().Match("/game/<some-value>?success=*");
            return;
        }
        GameId = new GameId(long.Parse(value));
        result.RequestUri().Should().Match($"/game/{GameId}?success=*");
    }

    private async Task PostJoinGame(string cookie) {
        var result = await _apiTester.As(cookie).PostForm("/lobby/join", new GameJoinRequest(GameId?.Value, null));
        result.StatusCode.Should().Be(200);
        result.RequestUri().Should().Match($"/game/{GameId}?success=*");
    }

    private async Task ViewGameJsonWithTurn(Color expectedTurn) {
        var json = await ViewGameJson();

        _apiTester.UnitOfWork.WithGameTransaction(tran => {
            var game = _apiTester.GameRepository.FindById(GameId!);

            var playerWithColor = game.Players.SingleOrDefault(p => p.Color == expectedTurn);
            json.Turn?.PlayerId.Should().Be(playerWithColor?.Id.Value);
        });
    }
    private async Task ViewGamePageWithVictor(string victor) {
        string page = await ViewGamePage();
        page.Should().Match($"*Victor: *{victor}*");
    }
    private async Task<string> ViewGamePage() {
        string page = await _apiTester.GetString($"/game/{GameId}");
        page.Should().Contain($"Game {GameId}</h1>");
        return page;
    }
    private async Task<GameDto> ViewGameJson() {
        var json = await _apiTester.GetJson<GameDto>($"/game/{GameId}/json");
        json.Should().NotBeNull();
        return json!;
    }

    private async Task PostMove(int from, int to, string cookie) {
        var result = await _apiTester.As(cookie).PostJson($"/game/{GameId}/move", new MoveRequest(from, to));
        result.StatusCode.Should().Be(200);
    }

    private async Task PostDrawVote(string cookie) {
        var result = await _apiTester.As(cookie).Post($"/game/{GameId}/draw");
        result.StatusCode.Should().Be(200);
        result.RequestUri().Should().Match($"/game/{GameId}?success=*");
    }

    private async Task PostResignation(string cookie) {
        var result = await _apiTester.As(cookie).Post($"/game/{GameId}/resign");
        result.StatusCode.Should().Be(200);
        result.RequestUri().Should().Match($"/game/{GameId}?success=*");
    }

    private void RecordUserStatistics() {
        _apiTester.UnitOfWork.WithUserTransaction(tran => {
            _recordedBlackUserStats = _apiTester.UserRepository.FindByName("TestPlayerBlack").Statistics;
            _recordedWhiteUserStats = _apiTester.UserRepository.FindByName("TestPlayerWhite").Statistics;
        });
    }

    private async Task AssertUserStatisticsDiff(Func<UserStatistics, int> blackStatFunc, int blackDiff,
            Func<UserStatistics, int> whiteStatFunc, int whiteDiff) {
        await _apiTester.WaitForEventsToComplete();

        if (_recordedBlackUserStats is null || _recordedWhiteUserStats is null) {
            throw new InvalidOperationException("Please record the user statistics before asserting.");
        }

        _apiTester.UnitOfWork.WithUserTransaction(tran => {
            var blackUser = _apiTester.UserRepository.FindByName("TestPlayerBlack");
            var whiteUser = _apiTester.UserRepository.FindByName("TestPlayerWhite");
            blackStatFunc(blackUser.Statistics).Should().Be(blackStatFunc(_recordedBlackUserStats) + blackDiff);
            whiteStatFunc(whiteUser.Statistics).Should().Be(whiteStatFunc(_recordedWhiteUserStats) + whiteDiff);
        });
    }

    private void AssertGameIsDraw() {
        _apiTester.UnitOfWork.WithGameTransaction(tran => {
            var game = _apiTester.GameRepository.FindById(GameId!);

            game.FinishedAt.Should().NotBeNull();
            game.Victor.Should().BeNull();
            game.Turn.Should().BeNull();
        });
    }
}
