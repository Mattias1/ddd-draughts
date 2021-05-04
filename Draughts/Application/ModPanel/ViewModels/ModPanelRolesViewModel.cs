using Draughts.Application.Shared.ViewModels;
using Draughts.Domain.AuthUserContext.Models;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Application.ModPanel.ViewModels {
    public class ModPanelRolesViewModel : ModPanelViewModel {
        public IReadOnlyList<RoleViewModel> Roles { get; }

        public ModPanelRolesViewModel(IReadOnlyList<Role> roles, MenuViewModel menuViewModel)
                : base (menuViewModel) {
            Roles = roles.Select(r => new RoleViewModel(r)).ToList().AsReadOnly();
        }
    }
}
