using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Repositories.InMemory;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using static Draughts.Application.ModPanel.ModPanelController;

namespace Draughts.IntegrationTest.EndToEnd.InMemory {
    // TODO: DB IT test, but without copy paste ;)
    public class InMemoryModPanelRoleIT {
        private readonly InMemoryApiTester _apiTest;
        private RoleId? _roleId;

        public InMemoryModPanelRoleIT() {
            _apiTest = new InMemoryApiTester();
        }

        [Fact]
        public async Task MessWithRoles() {
            _apiTest.LoginAsAdmin();

            await ViewModPanelOverviewPage();
            await ViewManageRolesPage();
            await PostCreateRole();

            await ViewEditRolePage();
            await PostEditRole();

            await ViewRoleUsersPage();
            await PostAssignUserToRole();
            await PostRemoveUserFromRole();
        }

        private async Task ViewModPanelOverviewPage() {
            string page = await _apiTest.GetString("/modpanel");
            page.Should().Contain("<h1>Mod panel - Overview</h1>");
        }

        private async Task ViewManageRolesPage() {
            string page = await _apiTest.GetString("/modpanel/roles");
            page.Should().Contain("<h1>Mod panel - Manage roles</h1>");
        }

        private async Task PostCreateRole() {
            var result = await _apiTest.PostForm("/modpanel/role/create", new CreateRoleRequest("New IT test role"));
            result.StatusCode.Should().Be(302);
            if (!_apiTest.TryRegex(result.Headers.Location?.ToString(), @"/modpanel/role/(\d+)/edit", out string? value)) {
                result.Headers.Location.Should().Be("/modpanel/role/<some-value>/edit");
                return;
            }
            _roleId = new RoleId(long.Parse(value));
            result.Headers.Location.Should().Be($"/modpanel/role/{_roleId}/edit");
        }

        private async Task ViewEditRolePage() {
            string page = await _apiTest.GetString($"/modpanel/role/{_roleId}/edit");
            page.Should().Contain("<h1>Mod panel - Edit role</h1>");
        }

        private async Task PostEditRole() {
            var result = await _apiTest.PostForm($"/modpanel/role/{_roleId}/edit",
                new EditRoleRequest("IT test role", new string[] { Permission.Permissions.PLAY_GAME }));
            result.StatusCode.Should().Be(302);
            result.Headers.Location.Should().Be("/modpanel/roles");
        }

        private async Task ViewRoleUsersPage() {
            string page = await _apiTest.GetString($"/modpanel/role/{_roleId}/users");
            page.Should().Contain("<h1>Mod panel - Role users</h1>");
        }

        private async Task PostAssignUserToRole() {
            var result = await _apiTest.PostForm($"/modpanel/role/{_roleId}/user",
                new AssignUserToRoleRequest("TestPlayerBlack"));
            result.StatusCode.Should().Be(302);
            result.Headers.Location.Should().Be($"/modpanel/role/{_roleId}/users");
        }

        private async Task PostRemoveUserFromRole() {
            long userId = UserDatabase.TestPlayerBlack;
            var result = await _apiTest.Post($"/modpanel/role/{_roleId}/user/{userId}/remove");
            result.StatusCode.Should().Be(302);
            result.Headers.Location.Should().Be($"/modpanel/role/{_roleId}/users");
        }

        // TODO: remove from in memory test, only add in DB test
        // private async Task PostDeleteRole() {
        //     var result = await _apiTest.Post($"/modpanel/role/{_roleId}/delete");
        //     result.StatusCode.Should().Be(302);
        //     result.Headers.Location.Should().Be("/modpanel/roles");
        // }
    }
}
