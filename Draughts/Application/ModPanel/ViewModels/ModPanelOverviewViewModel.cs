using Draughts.Application.Shared.ViewModels;
using Draughts.Domain.AuthUserAggregate.Models;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Application.ModPanel.ViewModels {
    public class ModPanelOverviewViewModel : ModPanelViewModel {
        public IReadOnlyList<AdminLogViewModel> AdminLogs { get; set; }

        public ModPanelOverviewViewModel(IReadOnlyList<AdminLog> adminLogs, MenuViewModel menuViewModel)
                : base(menuViewModel) {
            AdminLogs = adminLogs.Select(a => new AdminLogViewModel(a)).ToList().AsReadOnly();
        }
    }
}
