using Draughts.Application.Shared.ViewModels;
using Draughts.Domain.AuthUserAggregate.Models;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Application.ModPanel.ViewModels {
    public class ModpanelRolesViewModel : ModpanelViewModel {
        public IReadOnlyList<RoleViewModel> Roles { get; }

        public ModpanelRolesViewModel(IReadOnlyList<Role> roles, MenuViewModel menuViewModel)
                : base (menuViewModel) {
            Roles = roles.Select(r => new RoleViewModel(r)).ToList().AsReadOnly();
        }
    }
}
