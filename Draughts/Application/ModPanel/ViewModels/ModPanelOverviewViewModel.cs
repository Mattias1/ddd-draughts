using Draughts.Application.Shared.ViewModels;
using Draughts.Domain.AuthContext.Models;
using Draughts.Repositories.Misc;
using System.Collections.Generic;

namespace Draughts.Application.ModPanel.ViewModels;

public class ModPanelOverviewViewModel : ModPanelViewModel, IPaginationViewModel<AdminLogItemViewModel> {
    public IReadOnlyList<AdminLogItemViewModel> AdminLogs => Pagination.Results;
    public Pagination<AdminLogItemViewModel> Pagination { get; }

    public ModPanelOverviewViewModel(Pagination<AdminLog> adminLogs, MenuViewModel menuViewModel)
            : base(menuViewModel) {
        Pagination = adminLogs.Map(a => new AdminLogItemViewModel(a));
    }
}
