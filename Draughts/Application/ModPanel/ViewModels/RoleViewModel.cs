using Draughts.Domain.AuthUserAggregate.Models;
using NodaTime;
using System.Collections.Generic;

namespace Draughts.Application.ModPanel.ViewModels {
    public class RoleViewModel {
        public RoleId Id { get; }
        public string Rolename { get; }
        public IReadOnlyList<Permission> Permissions { get; }
        public ZonedDateTime CreatedAt { get; }

        public RoleViewModel(Role role) {
            Id = role.Id;
            Rolename = role.Rolename;
            Permissions = role.Permissions;
            CreatedAt = role.CreatedAt;
        }
    }
}
