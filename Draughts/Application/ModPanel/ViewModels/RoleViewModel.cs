using Draughts.Application.Shared.ViewModels;
using Draughts.Domain.AuthContext.Models;
using NodaTime;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Application.ModPanel.ViewModels;

public class RoleViewModel {
    public RoleId Id { get; }
    public string Rolename { get; }
    public int NrOfGrantedPermissions { get; }
    public IReadOnlyList<Role.PermissionItem> PermissionList { get; }
    public ZonedDateTime CreatedAt { get; }

    public RoleViewModel(Role role) {
        Id = role.Id;
        Rolename = role.Rolename;
        NrOfGrantedPermissions = role.Permissions.Count;
        PermissionList = role.BuildPermissionsList();
        CreatedAt = role.CreatedAt;
    }
}

public sealed class RoleWithUsersViewModel : RoleViewModel {
    public IReadOnlyList<BasicUserViewModel> AuthUsers { get; }

    public RoleWithUsersViewModel(Role role, IReadOnlyList<AuthUser> authUsers) : base(role) {
        AuthUsers = authUsers.Select(a => new BasicUserViewModel(a.Id, a.Username)).ToList().AsReadOnly();
    }
}
