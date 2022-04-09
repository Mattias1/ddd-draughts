using Draughts.Common;
using Draughts.Test.TestHelpers;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;
using static Draughts.Domain.AuthContext.Models.Permission;

namespace Draughts.Test.Domain.AuthContext;

public sealed class RoleTest {
    [Fact]
    public void ThrowWhenRoleNameIsTooShort() {
        var role = RoleTestHelper.RegisteredUser().Build();

        Action editingRole = () => role.Edit("42", role.Permissions);

        editingRole.Should().Throw<ManualValidationException>();
    }

    [Fact]
    public void EditRoleSuccessfully() {
        var role = RoleTestHelper.RegisteredUser().Build();

        role.Edit("1337", new[] { Permissions.ViewModPanel });

        role.Rolename.Should().Be("1337");
        role.Permissions.Should().BeEquivalentTo(new[] { Permissions.ViewModPanel });
    }

    [Fact]
    public void ThrowWhenModifyingAdminRoleName() {
        var role = RoleTestHelper.Admin().Build();

        Action editingRole = () => role.Edit("adMIN", role.Permissions);

        editingRole.Should().Throw<ManualValidationException>();
    }

    [Fact]
    public void ThrowWhenRemovingPermissionFromAdmin() {
        var role = RoleTestHelper.Admin().Build();
        var newPermissions = role.Permissions.Where(p => p != Permissions.PLAY_GAME).ToList();

        Action editingRole = () => role.Edit(role.Rolename, newPermissions);

        editingRole.Should().Throw<ManualValidationException>();
    }

    [Fact]
    public void AllowRemovingPendingRegistrationPermissionFromAdmin() {
        var role = RoleTestHelper.Admin()
            .AddPermission(Permissions.PendingRegistration)
            .Build();
        var newPermissions = role.Permissions.Where(p => p != Permissions.PENDING_REGISTRATION).ToList();

        role.Edit(role.Rolename, newPermissions);

        role.Permissions.Should().NotContain(Permissions.PendingRegistration);
    }

    [Fact]
    public void AllowAddingPermissionsToAdmin() {
        var role = RoleTestHelper.Admin().Build();
        var newPermissions = role.Permissions.Concat(new[] { Permissions.PendingRegistration }).ToList();

        role.Edit(role.Rolename, newPermissions);

        role.Permissions.Should().Contain(Permissions.PendingRegistration);
    }
}
