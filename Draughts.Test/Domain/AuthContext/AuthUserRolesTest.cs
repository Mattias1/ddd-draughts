using Draughts.Common;
using Draughts.Domain.AuthContext.Models;
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

    [Fact]
    public void ThrowWhenAssigningDuplicateRoles() {
        var authUser = AuthUserTestHelper.User().WithRoles(RegisteredRole).Build();
        var duplicateRole = RegisteredRole;

        Action assigning = () => authUser.AssignRole(duplicateRole.Id, duplicateRole.Rolename);

        assigning.Should().Throw<ManualValidationException>();
    }

    [Fact]
    public void AddRoleWhenAllIsWell() {
        var authUser = AuthUserTestHelper.User().WithRoles(RegisteredRole).Build();

        authUser.AssignRole(AdminRole.Id, AdminRole.Rolename);

        authUser.RoleIds.Should().BeEquivalentTo(new[] { RegisteredRole.Id, AdminRole.Id });
    }

    [Fact]
    public void ThrowWhenRemovingNonAssignedRole() {
        var authUser = AuthUserTestHelper.User().WithRoles(RegisteredRole).Build();
        var nonAssignedRole = AdminRole;

        Action removing = () => authUser.RemoveRole(nonAssignedRole.Id, nonAssignedRole.Rolename);

        removing.Should().Throw<ManualValidationException>();
    }

    [Fact]
    public void RemoveRoleWhenAllIsWell() {
        var authUser = AuthUserTestHelper.User().WithRoles(RegisteredRole, AdminRole).Build();

        authUser.RemoveRole(AdminRole.Id, AdminRole.Rolename);

        authUser.RoleIds.Should().BeEquivalentTo(new[] { RegisteredRole.Id });
    }

    [Fact]
    public void ThrowWhenRemovingAdminRoleFromAdmin() {
        var admin = AuthUserTestHelper.User(Username.ADMIN).WithRoles(RegisteredRole, AdminRole).Build();

        Action removing = () => admin.RemoveRole(AdminRole.Id, AdminRole.Rolename);

        removing.Should().Throw<ManualValidationException>();
    }
}
