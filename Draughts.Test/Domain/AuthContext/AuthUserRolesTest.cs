using Draughts.Common;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.AuthContext.Services;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using System;
using Xunit;

namespace Draughts.Test.Domain.AuthContext {
    public class AuthUserRolesTest {
        private static readonly Role RegisteredRole = RoleTestHelper.RegisteredUser().Build();
        private static readonly Role AdminRole = RoleTestHelper.Admin().Build();

        private readonly IUserRoleDomainService _userRoleService;

        public AuthUserRolesTest() {
            _userRoleService = new UserRoleDomainService();
        }

        [Fact]
        public void ThrowWhenAssigningDuplicateRoles() {
            var authUser = AuthUserTestHelper.User().WithRoles(RegisteredRole).Build();
            var duplicateRole = RegisteredRole;

            Action assigning = () => _userRoleService.AssignRole(authUser, duplicateRole);

            assigning.Should().Throw<ManualValidationException>();
        }

        [Fact]
        public void AddRoleWhenAllIsWell() {
            var authUser = AuthUserTestHelper.User().WithRoles(RegisteredRole).Build();

            _userRoleService.AssignRole(authUser, AdminRole);

            authUser.RoleIds.Should().BeEquivalentTo(RegisteredRole.Id, AdminRole.Id);
        }

        [Fact]
        public void ThrowWhenRemovingNonAssignedRole() {
            var authUser = AuthUserTestHelper.User().WithRoles(RegisteredRole).Build();
            var nonAssignedRole = AdminRole;

            Action removing = () => _userRoleService.RemoveRole(authUser, nonAssignedRole);

            removing.Should().Throw<ManualValidationException>();
        }

        [Fact]
        public void RemoveRoleWhenAllIsWell() {
            var authUser = AuthUserTestHelper.User().WithRoles(RegisteredRole, AdminRole).Build();

            _userRoleService.RemoveRole(authUser, AdminRole);

            authUser.RoleIds.Should().BeEquivalentTo(RegisteredRole.Id);
        }

        [Fact]
        public void ThrowWhenRemovingAdminRoleFromAdmin() {
            var admin = AuthUserTestHelper.User(Username.ADMIN).WithRoles(RegisteredRole, AdminRole).Build();

            Action removing = () => _userRoleService.RemoveRole(admin, AdminRole);

            removing.Should().Throw<ManualValidationException>();
        }
    }
}
