using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Repositories.InMemory;
using FluentAssertions;
using System.Threading.Tasks;
using static Draughts.Application.ModPanel.ModPanelController;

namespace Draughts.IntegrationTest.EndToEnd.Base {
    public class ModPanelRoleApiTester<T> where T : IApiTester {
        public T ApiTester { get; }
        public RoleId? RoleId { get; private set; }

        public ModPanelRoleApiTester(T apiTester) {
            ApiTester = apiTester;
        }

        public async Task ViewModPanelOverviewPage() {
            string page = await ApiTester.GetString("/modpanel");
            page.Should().Contain("<h1>Mod panel - Overview</h1>");
        }

        public async Task ViewManageRolesPage() {
            string page = await ApiTester.GetString("/modpanel/roles");
            page.Should().Contain("<h1>Mod panel - Manage roles</h1>");
        }

        public async Task PostCreateRole() {
            var result = await ApiTester.PostForm("/modpanel/role/create", new CreateRoleRequest("New IT test role"));
            result.StatusCode.Should().Be(302);
            if (!ApiTester.TryRegex(result.Headers.Location?.ToString(), @"/modpanel/role/(\d+)/edit", out string? value)) {
                result.Headers.Location.Should().Be("/modpanel/role/<some-value>/edit");
                return;
            }
            RoleId = new RoleId(long.Parse(value));
            result.Headers.Location.Should().Be($"/modpanel/role/{RoleId}/edit");
        }

        public async Task ViewEditRolePage() {
            string page = await ApiTester.GetString($"/modpanel/role/{RoleId}/edit");
            page.Should().Contain("<h1>Mod panel - Edit role</h1>");
        }

        public async Task PostEditRole() {
            var result = await ApiTester.PostForm($"/modpanel/role/{RoleId}/edit",
                new EditRoleRequest("IT test role", new string[] { Permission.Permissions.PLAY_GAME }));
            result.StatusCode.Should().Be(302);
            result.Headers.Location.Should().Be("/modpanel/roles");
        }

        public async Task ViewRoleUsersPage() {
            string page = await ApiTester.GetString($"/modpanel/role/{RoleId}/users");
            page.Should().Contain("<h1>Mod panel - Role users</h1>");
        }

        public async Task PostAssignUserToRole() {
            var result = await ApiTester.PostForm($"/modpanel/role/{RoleId}/user",
                new AssignUserToRoleRequest("TestPlayerBlack"));
            result.StatusCode.Should().Be(302);
            result.Headers.Location.Should().Be($"/modpanel/role/{RoleId}/users");
        }

        public async Task PostRemoveUserFromRole() {
            long userId = UserDatabase.TestPlayerBlack;
            var result = await ApiTester.Post($"/modpanel/role/{RoleId}/user/{userId}/remove");
            result.StatusCode.Should().Be(302);
            result.Headers.Location.Should().Be($"/modpanel/role/{RoleId}/users");
        }

        public async Task PostDeleteRole() {
            var result = await ApiTester.Post($"/modpanel/role/{RoleId}/delete");
            result.StatusCode.Should().Be(302);
            result.Headers.Location.Should().Be("/modpanel/roles");
        }
    }
}
