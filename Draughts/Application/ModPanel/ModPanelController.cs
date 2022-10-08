using Draughts.Application.ModPanel.ViewModels;
using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Draughts.Common.Events.Specifications;
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
    private readonly EventsRepository _eventsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ModPanelController(AdminLogRepository adminLogRepository, EventsRepository eventsRepository,
            IUnitOfWork unitOfWork) {
        _adminLogRepository = adminLogRepository;
        _eventsRepository = eventsRepository;
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
        var authEvents = _unitOfWork.WithAuthTransaction(tran => {
            return _eventsRepository.PaginateSentEvents(page, 20, new EventIdSort());
        });
        var userEvents = _unitOfWork.WithUserTransaction(tran => {
            return _eventsRepository.PaginateSentEvents(page, 20, new EventIdSort());
        });
        var gameEvents = _unitOfWork.WithGameTransaction(tran => {
            return _eventsRepository.PaginateSentEvents(page, 20, new EventIdSort());
        });
        return View(new EventQueueViewModel(authEvents, userEvents, gameEvents));
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
