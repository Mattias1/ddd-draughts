using Draughts.Application.Shared.ViewModels;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Repositories;
using System.Collections.Generic;

namespace Draughts.Application.ModPanel.ViewModels {
    public class ModPanelOverviewViewModel : ModPanelViewModel, IPaginationViewModel<AdminLogViewModel> {
        public IReadOnlyList<AdminLogViewModel> AdminLogs => Pagination.Results;
        public Pagination<AdminLogViewModel> Pagination { get; }

        public ModPanelOverviewViewModel(Pagination<AdminLog> adminLogs, MenuViewModel menuViewModel)
                : base(menuViewModel) {
            Pagination = adminLogs.Map(a => new AdminLogViewModel(a));
        }
    }
}
