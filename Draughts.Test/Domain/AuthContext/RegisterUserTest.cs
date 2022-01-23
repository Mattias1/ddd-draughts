using Draughts.Common;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.AuthContext.Services;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using NodaTime.Testing;
using System;
using Xunit;

namespace Draughts.Test.Domain.AuthContext {
    public class RegisterUserTest {
        private static readonly Role PendingRole = RoleTestHelper.PendingRegistration().Build();
        private static readonly Role RegisteredRole = RoleTestHelper.RegisteredUser().Build();

        private readonly IUserRegistrationDomainService _userRegistrationService;

        public RegisterUserTest() {
            var clock = FakeClock.FromUtc(2020, 02, 29);
            var userRoleService = new UserRoleDomainService();
            _userRegistrationService = new UserRegistrationDomainService(clock, userRoleService);
        }

        [Fact]
        public void ThrowWhenNotPassingRegisteredUserRole() {
            var authUser = AuthUserTestHelper.User().WithRoles(PendingRole).Build();
            var wrongRole = PendingRole;

            Action registration = () => _userRegistrationService.Register(authUser, wrongRole, PendingRole);

            registration.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowWhenNotPassingPendingRole() {
            var authUser = AuthUserTestHelper.User().WithRoles(PendingRole).Build();
            var wrongRole = RegisteredRole;

            Action registration = () => _userRegistrationService.Register(authUser, RegisteredRole, wrongRole);

            registration.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ValidationErrorWhenUserHasNoRoles() {
            var authUser = AuthUserTestHelper.User().WithRoles().Build();

            Action registration = () => _userRegistrationService.Register(authUser, RegisteredRole, PendingRole);

            registration.Should().Throw<ManualValidationException>();
        }

        [Fact]
        public void ValidationErrorWhenDoesntHavePendingRole() {
            var wrongRole = RegisteredRole;
            var authUser = AuthUserTestHelper.User().WithRoles(wrongRole).Build();

            Action registration = () => _userRegistrationService.Register(authUser, RegisteredRole, PendingRole);

            registration.Should().Throw<ManualValidationException>();
        }

        [Fact]
        public void SwapOutRolesWhenAllIsWell() {
            var authUser = AuthUserTestHelper.User().WithRoles(PendingRole).Build();

            _userRegistrationService.Register(authUser, RegisteredRole, PendingRole);

            authUser.RoleIds.Should().BeEquivalentTo(new[] { RegisteredRole.Id });
        }
    }
}
