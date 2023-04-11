using Draughts.Domain.AuthContext.Models;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using static Draughts.Application.ModPanel.ModPanelRolesController;

namespace Draughts.IntegrationTest.EndToEnd;

public sealed class ModPanelRoleIT {
    private readonly ApiTester _apiTester;

    private const string EDITED_ROLENAME = "IT test role";
    private const string ASSIGNED_USERNAME = "TestPlayerBlack";
    private const long TEST_PLAYER_BLACK_USER_ID = 8;

    private RoleId? RoleId;

    public ModPanelRoleIT() {
        _apiTester = new ApiTester();
    }

    [Fact]
    public async Task MessWithRoles() {
        _apiTester.LoginAsAdmin();

        await ViewModPanelOverviewPage();
        await ViewManageRolesPage();
        await PostCreateRole();

        await ViewEditRolePage();
        await PostEditRole();

        await ViewRoleUsersPage();
        await PostAssignUserToRole();

        AssertRoleIsCorrect();

        await PostRemoveUserFromRole();
        await PostDeleteRole();
    }

    private async Task ViewModPanelOverviewPage() {
        string page = await _apiTester.GetString("/modpanel");
        page.Should().Contain("<h1>Mod panel - Overview</h1>");
    }

    private async Task ViewManageRolesPage() {
        string page = await _apiTester.GetString("/modpanel/roles");
        page.Should().Contain("<h1>Mod panel - Manage roles</h1>");
    }

    private async Task PostCreateRole() {
        var result = await _apiTester.PostForm("/modpanel/role/create", new CreateRoleRequest("New IT test role"));
        result.Should().HaveStatusCode(200);
        if (!_apiTester.TryRegex(result.RequestUri(), @"/modpanel/role/(\d+)/edit", out string? value)) {
            // Make the test fail with a nice message.
            result.RequestUri().Should().Match("/modpanel/role/<some-value>/edit?success=*");
            return;
        }
        RoleId = new RoleId(long.Parse(value));
        result.RequestUri().Should().Match($"/modpanel/role/{RoleId}/edit?success=*");

        AssertRoleIsLogged("role.create");
    }

    private async Task ViewEditRolePage() {
        string page = await _apiTester.GetString($"/modpanel/role/{RoleId}/edit");
        page.Should().Contain("<h1>Mod panel - Edit role</h1>");
    }

    private async Task PostEditRole() {
        var result = await _apiTester.PostForm($"/modpanel/role/{RoleId}/edit",
            new EditRoleRequest(EDITED_ROLENAME, new string[] { Permission.Permissions.PLAY_GAME }));
        result.Should().HaveStatusCode(200);
        result.RequestUri().Should().Match("/modpanel/roles?success=*");

        AssertRoleIsLogged("role.edit");
    }

    private async Task ViewRoleUsersPage() {
        string page = await _apiTester.GetString($"/modpanel/role/{RoleId}/users");
        page.Should().Contain("<h1>Mod panel - Role users</h1>");
    }

    private async Task PostAssignUserToRole() {
        var result = await _apiTester.PostForm($"/modpanel/role/{RoleId}/user",
            new AssignUserToRoleRequest(ASSIGNED_USERNAME));
        result.Should().HaveStatusCode(200);
        result.RequestUri().Should().Match($"/modpanel/role/{RoleId}/users?success=*");

        AssertRoleIsLogged("role.gain");
    }

    private async Task PostRemoveUserFromRole() {
        long userId = TEST_PLAYER_BLACK_USER_ID;
        var result = await _apiTester.Post($"/modpanel/role/{RoleId}/user/{userId}/remove");
        result.Should().HaveStatusCode(200);
        result.RequestUri().Should().Match($"/modpanel/role/{RoleId}/users?success=*");

        AssertRoleIsLogged("role.lose");
    }

    private async Task PostDeleteRole() {
        var result = await _apiTester.Post($"/modpanel/role/{RoleId}/delete");
        result.Should().HaveStatusCode(200);
        result.RequestUri().Should().Match("/modpanel/roles?success=*");

        AssertRoleIsLogged("role.delete");
    }

    private void AssertRoleIsLogged(string adminLogType) {
        _apiTester.UnitOfWork.WithAuthTransaction(tran => {
            var adminLogs = _apiTester.AdminLogRepository.List();
            adminLogs.Should().Contain(l => l.Type == adminLogType, $"a '{adminLogType}' type event has happened.");
        });
    }

    private void AssertRoleIsCorrect() {
        _apiTester.UnitOfWork.WithAuthTransaction(tran => {
            var role = _apiTester.RoleRepository.FindById(RoleId!);
            role.Rolename.Should().Be(EDITED_ROLENAME);
            role.Permissions.Should().BeEquivalentTo(new[] { Permission.Permissions.PlayGame });

            var authUser = _apiTester.AuthUserRepository.FindByName(ASSIGNED_USERNAME);
            authUser.RoleIds.Should().Contain(role.Id);
        });
    }
}
