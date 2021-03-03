using Draughts.Common;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using System;
using Xunit;

namespace Draughts.Test.Domain.AuthUserAggregate {
    public class AuthUserRolesTest {
        private static readonly Role RegisteredRole = RoleTestHelper.RegisteredUser().Build();
        private static readonly Role AdminRole = RoleTestHelper.Admin().Build();

        [Fact]
        public void ThrowWhenAssigningDuplicateRoles() {
            var authUser = AuthUserTestHelper.User().WithRoles(RegisteredRole).Build();
            var duplicateRole = RegisteredRole;

            Action registration = () => authUser.AssignRole(duplicateRole);

            registration.Should().Throw<ManualValidationException>();
        }

        [Fact]
        public void AddRoleWhenAllIsWell() {
            var authUser = AuthUserTestHelper.User().WithRoles(RegisteredRole).Build();

            var roles = authUser.Roles;
            authUser.AssignRole(AdminRole);

            roles.Should().BeEquivalentTo(RegisteredRole, AdminRole);
        }

        [Fact]
        public void ThrowWhenRemovingNonAssignedRole() {
            var authUser = AuthUserTestHelper.User().WithRoles(RegisteredRole).Build();
            var nonAssignedRole = AdminRole;

            Action registration = () => authUser.RemoveRole(nonAssignedRole);

            registration.Should().Throw<ManualValidationException>();
        }

        [Fact]
        public void RemoveRoleWhenAllIsWell() {
            var authUser = AuthUserTestHelper.User().WithRoles(RegisteredRole, AdminRole).Build();

            var roles = authUser.Roles;
            authUser.RemoveRole(AdminRole);

            roles.Should().BeEquivalentTo(RegisteredRole);
        }
    }
}
