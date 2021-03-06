using Draughts.Domain.AuthUserContext.Models;
using Draughts.Repositories.InMemory;
using FluentAssertions;
using System.Threading.Tasks;
using static Draughts.Application.ModPanel.ModPanelController;

namespace Draughts.IntegrationTest.EndToEnd.Base {
    public class ModPanelRoleTesterApi<T> where T : BaseApiTester {
        public T ApiTester { get; }
        public RoleId? RoleId { get; private set; }

        public ModPanelRoleTesterApi(T apiTester) {
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
            if (!ApiTester.TryRegex(result.RedirectLocation(), @"/modpanel/role/(\d+)/edit", out string? value)) {
                result.RedirectLocation().Should().Match("/modpanel/role/<some-value>/edit?success=*");
                return;
            }
            RoleId = new RoleId(long.Parse(value));
            result.RedirectLocation().Should().Match($"/modpanel/role/{RoleId}/edit?success=*");
        }

        public async Task ViewEditRolePage() {
            string page = await ApiTester.GetString($"/modpanel/role/{RoleId}/edit");
            page.Should().Contain("<h1>Mod panel - Edit role</h1>");
        }

        public async Task PostEditRole() {
            var result = await ApiTester.PostForm($"/modpanel/role/{RoleId}/edit",
                new EditRoleRequest("IT test role", new string[] { Permission.Permissions.PLAY_GAME }));
            result.StatusCode.Should().Be(302);
            result.RedirectLocation().Should().Match("/modpanel/roles?success=*");
        }

        public async Task ViewRoleUsersPage() {
            string page = await ApiTester.GetString($"/modpanel/role/{RoleId}/users");
            page.Should().Contain("<h1>Mod panel - Role users</h1>");
        }

        public async Task PostAssignUserToRole() {
            var result = await ApiTester.PostForm($"/modpanel/role/{RoleId}/user",
                new AssignUserToRoleRequest("TestPlayerBlack"));
            result.StatusCode.Should().Be(302);
            result.RedirectLocation().Should().Match($"/modpanel/role/{RoleId}/users?success=*");
        }

        public async Task PostRemoveUserFromRole() {
            long userId = UserDatabase.TestPlayerBlack;
            var result = await ApiTester.Post($"/modpanel/role/{RoleId}/user/{userId}/remove");
            result.StatusCode.Should().Be(302);
            result.RedirectLocation().Should().Match($"/modpanel/role/{RoleId}/users?success=*");
        }

        public async Task PostDeleteRole() {
            var result = await ApiTester.Post($"/modpanel/role/{RoleId}/delete");
            result.StatusCode.Should().Be(302);
            result.RedirectLocation().Should().Match("/modpanel/roles?success=*");
        }
    }
}
