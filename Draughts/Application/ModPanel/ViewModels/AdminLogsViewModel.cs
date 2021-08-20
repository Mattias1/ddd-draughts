using Draughts.Application.Shared.ViewModels;
using Draughts.Domain.AuthContext.Models;
using Draughts.Repositories;
using System.Collections.Generic;

namespace Draughts.Application.ModPanel.ViewModels {
    public class AdminLogsViewModel : IPaginationViewModel<AdminLogViewModel> {
        public IReadOnlyList<AdminLogViewModel> AdminLogs => Pagination.Results;
        public Pagination<AdminLogViewModel> Pagination { get; }

        public AdminLogsViewModel(Pagination<AdminLog> adminLogs) {
            Pagination = adminLogs.Map(a => new AdminLogViewModel(a));
        }
    }
}
