using Draughts.Domain.AuthUserAggregate.Models;
using NodaTime;
using System.Collections.Generic;

namespace Draughts.Application.ModPanel.ViewModels {
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
}
