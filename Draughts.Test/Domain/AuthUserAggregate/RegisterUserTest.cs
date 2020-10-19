using Draughts.Common;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using System;
using Xunit;

namespace Draughts.Test.Domain.AuthUserAggregate {
    public class RegisterUserTest {
        private static readonly Role PendingRole = RoleTestHelper.PendingRegistration().Build();
        private static readonly Role RegisteredRole = RoleTestHelper.RegisteredUser().Build();

        [Fact]
        public void Register_WhenRoleIsNotRegisteredUser_ThenThrow() {
            var authUser = AuthUserTestHelper.User().WithRoles(PendingRole).Build();
            var wrongRole = PendingRole;

            Action registration = () => authUser.Register(wrongRole);

            registration.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Register_WhenUserHasNoRoles_ThenValidationError() {
            var authUser = AuthUserTestHelper.User().WithRoles().Build();

            Action registration = () => authUser.Register(RegisteredRole);

            registration.Should().Throw<ManualValidationException>();
        }

        [Fact]
        public void Register_WhenUserHasMultipleRoles_ThenValidationError() {
            var authUser = AuthUserTestHelper.User().WithRoles(PendingRole, RegisteredRole).Build();

            Action registration = () => authUser.Register(RegisteredRole);

            registration.Should().Throw<ManualValidationException>();
        }

        [Fact]
        public void Register_WhenRoleIsNotPending_ThenValidationError() {
            var wrongRole = RegisteredRole;
            var authUser = AuthUserTestHelper.User().WithRoles(wrongRole).Build();

            Action registration = () => authUser.Register(RegisteredRole);

            registration.Should().Throw<ManualValidationException>();
        }

        [Fact]
        public void Register_WhenAllIsWell_ThenSwapOutRoles() {
            var authUser = AuthUserTestHelper.User().WithRoles(PendingRole).Build();

            authUser.Register(RegisteredRole);

            authUser.Roles.Should().OnlyContain(r => r == RegisteredRole);
        }
    }
}
