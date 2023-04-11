using Draughts.Common.Utilities;
using Draughts.Domain.GameContext.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using NodaTime;
using System.Threading.Tasks;
using Xunit;
using static Draughts.Application.ModPanel.ModPanelGameToolsController;

namespace Draughts.IntegrationTest.EndToEnd;

public sealed class ModPanelGameToolsIT {
    private readonly ApiTester _apiTester;
    private GameId? _gameId;

    public ModPanelGameToolsIT() {
        _apiTester = new ApiTester();
    }

    [Fact]
    public async Task MessWithRoles() {
        CreateGame();

        _apiTester.LoginAsAdmin();

        await ViewModPanelOverviewPage();
        await ViewManageRolesPage();
        await PostChangeTurnTime();

        AssertGameTimeIsChanged();
    }

    private void CreateGame() {
        _apiTester.UnitOfWork.WithGameTransaction(tran => {
            var game = GameTestHelper.StartedInternationalGame()
                .WithTurn(Color.White, _apiTester.Clock.UtcNow())
                .Build();
            _apiTester.GameRepository.Save(game);
            _gameId = game.Id;
        });
    }

    private async Task ViewModPanelOverviewPage() {
        string page = await _apiTester.GetString("/modpanel");
        page.Should().Contain("<h1>Mod panel - Overview</h1>");
    }

    private async Task ViewManageRolesPage() {
        string page = await _apiTester.GetString("/modpanel/game-tools");
        page.Should().Contain("<h1>Mod panel - Game tools</h1>");
    }

    private async Task PostChangeTurnTime() {
        int turnTime36Hours = 129600;
        var result = await _apiTester.PostForm("/modpanel/game-tools/turn-time",
            new ChangeTurnTimeRequest(_gameId?.Value, turnTime36Hours, true));
        result.Should().HaveStatusCode(200);
        result.RequestUri().Should().Match("/modpanel/game-tools?success=*");

        AssertActionIsLogged("game.turntimechange");
    }

    private void AssertActionIsLogged(string adminLogType) {
        _apiTester.UnitOfWork.WithAuthTransaction(tran => {
            var adminLogs = _apiTester.AdminLogRepository.List();
            adminLogs.Should().Contain(l => l.Type == adminLogType, $"a '{adminLogType}' type event has happened.");
        });
    }

    private void AssertGameTimeIsChanged() {
        var now = _apiTester.Clock.GetCurrentInstant();

        _gameId.Should().NotBeNull();
        _apiTester.UnitOfWork.WithGameTransaction(tran => {
            var game = _apiTester.GameRepository.FindById(_gameId!);
            bool turnTimeIsCloseTo36Hours = game.Turn!.ExpiresAt.ToInstant() > now.Plus(Duration.FromHours(35));
            turnTimeIsCloseTo36Hours.Should().BeTrue();
        });
    }
}
