using Draughts.Application.ModPanel.Services;
using Draughts.Application.ModPanel.ViewModels;
using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.AuthContext.Specifications;
using Draughts.Repositories;
using Draughts.Repositories.Misc;
using Draughts.Repositories.Transaction;
using Microsoft.AspNetCore.Mvc;
using static Draughts.Domain.AuthContext.Models.Permission;

namespace Draughts.Application.ModPanel;

public sealed class ModPanelController : BaseController {
    private readonly AdminLogRepository _adminLogRepository;
    private readonly SystemEventQueueService _systemEventQueueService;
    private readonly IUnitOfWork _unitOfWork;

    public ModPanelController(AdminLogRepository adminLogRepository, SystemEventQueueService systemEventQueueService,
            IUnitOfWork unitOfWork) {
        _adminLogRepository = adminLogRepository;
        _systemEventQueueService = systemEventQueueService;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("/modpanel"), Requires(Permissions.VIEW_MOD_PANEL)]
    public IActionResult ModPanel() {
        var adminLogs = GetAdminLogs(1, 10);
        return View(new ModPanelOverviewViewModel(adminLogs, BuildMenu()));
    }

    [HttpGet("/modpanel/admin-logs"), Requires(Permissions.VIEW_ADMIN_LOGS)]
    public IActionResult AdminLogs(int page = 1) {
        var adminLogs = GetAdminLogs(page, 20);
        return View(new AdminLogsViewModel(adminLogs));
    }

    private Pagination<AdminLog> GetAdminLogs(int page, int pageSize) {
        var adminLogs = _unitOfWork.WithAuthTransaction(tran => {
            return _adminLogRepository.Paginate(page, pageSize, new AdminLogIdSort());
        });
        return adminLogs;
    }

    [HttpGet("/modpanel/event-queue"), Requires(Permissions.SYSTEM_MAINTENANCE)]
    public IActionResult EventQueue(int page = 1) {
        var (authEvents, userEvents, gameEvents) = _systemEventQueueService.ViewEventQueues(page, 20);
        return View(new EventQueueViewModel(authEvents, userEvents, gameEvents));
    }

    [HttpPost("/modpanel/event-queue/sync-status"), Requires(Permissions.SYSTEM_MAINTENANCE)]
    public IActionResult SyncEventQueueStatus() {
        _systemEventQueueService.SyncEventQueueStatus(AuthContext.UserId, AuthContext.Username);
        return SuccessRedirect("/modpanel/event-queue", "Synced status of all events.");
    }

    [HttpPost("/modpanel/event-queue/redispatch"), Requires(Permissions.SYSTEM_MAINTENANCE)]
    public IActionResult RedispatchEventQueue() {
        _systemEventQueueService.RedispatchEventQueue(AuthContext.UserId, AuthContext.Username);
        return SuccessRedirect("/modpanel/event-queue", "Re-dispatched all events.");
    }

    public static MenuViewModel BuildMenu() {
        return new MenuViewModel("Mod panel",
            ("Overview", "/modpanel"),
            ("Game tools", "/modpanel/game-tools"),
            ("Manage roles", "/modpanel/roles"),
            ("Event queue", "/modpanel/event-queue")
        );
    }
}
