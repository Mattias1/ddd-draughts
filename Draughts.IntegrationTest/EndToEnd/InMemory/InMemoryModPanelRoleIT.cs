using Draughts.IntegrationTest.EndToEnd.Base;
using System.Threading.Tasks;
using Xunit;

namespace Draughts.IntegrationTest.EndToEnd.InMemory;

[Collection("ModPanelRoleIT")]
public class InMemoryModPanelRoleIT {
    private readonly InMemoryApiTester _apiTester;
    private readonly ModPanelRoleTesterApi<InMemoryApiTester> _modPanelApi;

    public InMemoryModPanelRoleIT() {
        _apiTester = new InMemoryApiTester();
        _modPanelApi = new ModPanelRoleTesterApi<InMemoryApiTester>(_apiTester);
    }

    [Fact]
    public async Task MessWithRoles() {
        _apiTester.LoginAsAdmin();

        await _modPanelApi.ViewModPanelOverviewPage();
        await _modPanelApi.ViewManageRolesPage();
        await _modPanelApi.PostCreateRole();

        await _modPanelApi.ViewEditRolePage();
        await _modPanelApi.PostEditRole();

        await _modPanelApi.ViewRoleUsersPage();
        await _modPanelApi.PostAssignUserToRole();

        _modPanelApi.AssertRoleIsCorrect();

        await _modPanelApi.PostRemoveUserFromRole();
    }
}
