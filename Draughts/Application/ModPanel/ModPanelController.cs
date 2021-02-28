using Draughts.Application.ModPanel.ViewModels;
using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;

namespace Draughts.Application.ModPanel {
    [Requires(Permissions.VIEW_MOD_PANEL)]
    public class ModPanelController : BaseController {
        private readonly IAdminLogRepository _adminLogRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ModPanelController(IAdminLogRepository adminLogRepository,
                IRoleRepository roleRepository, IUnitOfWork unitOfWork) {
            _adminLogRepository = adminLogRepository;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("/modpanel")]
        public IActionResult ModPanel() {
            var adminLogs = GetAdminLogs();
            return View(new ModpanelOverviewViewModel(adminLogs, BuildMenu()));
        }

        [HttpGet("/modpanel/game-tools")]
        public IActionResult GameTools() {
            return View(new ModpanelViewModel(BuildMenu()));
        }

        [HttpGet("/modpanel/roles")]
        public IActionResult ManageRoles() {
            return View(new ModpanelViewModel(BuildMenu()));
        }

        [HttpGet("/modpanel/admin-logs")]
        public IActionResult AdminLogs() {
            var adminLogs = GetAdminLogs();
            return View(new ModpanelOverviewViewModel(adminLogs, BuildMenu()));
        }

        private IReadOnlyList<AdminLog> GetAdminLogs() {
            var adminLogs = _unitOfWork.WithAuthUserTransaction(tran => {
                var adminLogs = _adminLogRepository.List(); // TODO: Only the last 10 logrows or something.
                return tran.CommitWith(adminLogs);
            });
            return adminLogs;
        }

        private MenuViewModel BuildMenu() {
            return new MenuViewModel("Modpanel",
                ("Game tools", "/modpanel/game-tools"),
                ("Manage roles", "/modpanel/roles")
            );
        }
    }
}
