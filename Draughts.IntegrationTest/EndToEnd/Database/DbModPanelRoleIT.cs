using Draughts.IntegrationTest.EndToEnd.Base;
using System.Threading.Tasks;
using Xunit;

namespace Draughts.IntegrationTest.EndToEnd.Database {
    public class DbModPanelRoleIT {
        private readonly DbApiTester _apiTester;
        private readonly ModPanelRoleApiTester<DbApiTester> _modPanelApi;

        public DbModPanelRoleIT() {
            _apiTester = new DbApiTester();
            _modPanelApi = new ModPanelRoleApiTester<DbApiTester>(_apiTester);
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
            await _modPanelApi.PostRemoveUserFromRole();

            await _modPanelApi.PostDeleteRole();
        }
    }
}
