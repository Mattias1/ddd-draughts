using Draughts.Common;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.AuthContext.Services;
using Draughts.Domain.UserContext.Models;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using System;
using Xunit;

namespace Draughts.Test.Domain.AuthContext;

public sealed class AuthUserRolesTest {
    private static readonly UserId AdminUserId = new UserId(1);
    private static readonly Role RegisteredRole = RoleTestHelper.RegisteredUser().Build();
    private static readonly Role AdminRole = RoleTestHelper.Admin().Build();

    private readonly UserRoleDomainService _userRoleService;

    public AuthUserRolesTest() {
        _userRoleService = new UserRoleDomainService();
    }

    [Fact]
    public void ThrowWhenAssigningDuplicateRoles() {
        var authUser = AuthUserTestHelper.User().WithRoles(RegisteredRole).Build();
        var duplicateRole = RegisteredRole;

        Action assigning = () => _userRoleService.AssignRole(authUser, duplicateRole, AdminUserId);

        assigning.Should().Throw<ManualValidationException>();
    }

    [Fact]
    public void AddRoleWhenAllIsWell() {
        var authUser = AuthUserTestHelper.User().WithRoles(RegisteredRole).Build();

        _userRoleService.AssignRole(authUser, AdminRole, AdminUserId);

        authUser.RoleIds.Should().BeEquivalentTo(new[] { RegisteredRole.Id, AdminRole.Id });
    }

    [Fact]
    public void ThrowWhenRemovingNonAssignedRole() {
        var authUser = AuthUserTestHelper.User().WithRoles(RegisteredRole).Build();
        var nonAssignedRole = AdminRole;

        Action removing = () => _userRoleService.RemoveRole(authUser, nonAssignedRole, AdminUserId);

        removing.Should().Throw<ManualValidationException>();
    }

    [Fact]
    public void RemoveRoleWhenAllIsWell() {
        var authUser = AuthUserTestHelper.User().WithRoles(RegisteredRole, AdminRole).Build();

        _userRoleService.RemoveRole(authUser, AdminRole, AdminUserId);

        authUser.RoleIds.Should().BeEquivalentTo(new[] { RegisteredRole.Id });
    }

    [Fact]
    public void ThrowWhenRemovingAdminRoleFromAdmin() {
        var admin = AuthUserTestHelper.User(Username.ADMIN).WithRoles(RegisteredRole, AdminRole).Build();

        Action removing = () => _userRoleService.RemoveRole(admin, AdminRole, AdminUserId);

        removing.Should().Throw<ManualValidationException>();
    }
}
