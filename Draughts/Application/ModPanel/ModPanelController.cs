using Draughts.Application.ModPanel.Services;
using Draughts.Application.ModPanel.ViewModels;
using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Draughts.Common;
using Draughts.Common.Utilities;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.AuthContext.Specifications;
using Draughts.Domain.GameContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using static Draughts.Domain.AuthContext.Models.Permission;

namespace Draughts.Application.ModPanel;

public class ModPanelController : BaseController {
    private readonly IAdminLogRepository _adminLogRepository;
    private readonly IClock _clock;
    private readonly EditRoleService _editRoleService;
    private readonly IGameRepository _gameRepository;
    private readonly RoleUsersService _roleUserService;
    private readonly IUnitOfWork _unitOfWork;

    public ModPanelController(IAdminLogRepository adminLogRepository, IClock clock, EditRoleService editRoleService,
            IGameRepository gameRepository, RoleUsersService roleUsersService, IUnitOfWork unitOfWork) {
        _adminLogRepository = adminLogRepository;
        _clock = clock;
        _editRoleService = editRoleService;
        _gameRepository = gameRepository;
        _roleUserService = roleUsersService;
        _unitOfWork = unitOfWork;
    }

    // --- Overview ---
    [HttpGet("/modpanel"), Requires(Permissions.VIEW_MOD_PANEL)]
    public IActionResult ModPanel() {
        var adminLogs = GetAdminLogs(1, 10);
        return View(new ModPanelOverviewViewModel(adminLogs, BuildMenu()));
    }

    // --- Game tools ---
    [HttpGet("/modpanel/game-tools"), Requires(Permissions.EDIT_GAMES)]
    public IActionResult GameTools() {
        return View(new ModPanelViewModel(BuildMenu()));
    }

    [HttpPost("/modpanel/game-tools/turn-time"), Requires(Permissions.EDIT_GAMES)]
    public IActionResult ChangeTurnTime([FromForm] ChangeTurnTimeRequest? request) {
        try {
            ValidateNotNull(request?.GameId, request?.TurnTimeInSeconds);

            _unitOfWork.WithGameTransaction(tran => {
                var game = _gameRepository.FindById(new GameId(request?.GameId));
                game.ChangeTurnTime(_clock.UtcNow(), request!.TurnTimeInSeconds!.Value, request.ForAllFutureTurns ?? false);
                _gameRepository.Save(game);
            });

            return SuccessRedirect($"/modpanel/game-tools", $"Turn time for game {request?.GameId} is changed.");
        }
        catch (ManualValidationException e) {
            return ErrorRedirect($"/modpanel/game-tools", e.Message);
        }
    }

    // --- Manage roles ---
    [HttpGet("/modpanel/roles"), Requires(Permissions.EDIT_ROLES)]
    public IActionResult ManageRoles() {
        var roles = _editRoleService.GetRoles();
        return View(new ModPanelRolesViewModel(roles, BuildMenu()));
    }

    // --- Edit role ---
    [HttpGet("/modpanel/role/{roleId:long}/edit"), Requires(Permissions.EDIT_ROLES)]
    public IActionResult EditRole(long roleId) {
        try {
            var role = _editRoleService.GetRole(new RoleId(roleId));
            return View(new RoleViewModel(role));
        }
        catch (ManualValidationException e) {
            return ErrorRedirect("/modpanel/roles", e.Message);
        }
    }

    [HttpPost("/modpanel/role/create"), Requires(Permissions.EDIT_ROLES)]
    public IActionResult CreateRole([FromForm] CreateRoleRequest? request) {
        try {
            ValidateNotNull(request?.Rolename);

            var role = _editRoleService.CreateRole(AuthContext.UserId, request!.Rolename!);

            return SuccessRedirect($"/modpanel/role/{role.Id}/edit", $"Role '{role.Rolename}' is added.");
        }
        catch (ManualValidationException e) {
            return ErrorRedirect($"/modpanel/roles", e.Message);
        }
    }

    [HttpPost("/modpanel/role/{roleId:long}/edit"), Requires(Permissions.EDIT_ROLES)]
    public IActionResult EditRolePost(long roleId, [FromForm] EditRoleRequest? request) {
        try {
            ValidateNotNull(request?.Rolename, request?.Permissions);

            _editRoleService.EditRole(AuthContext.UserId, new RoleId(roleId),
                request!.Rolename!, request.Permissions!);

            return SuccessRedirect("/modpanel/roles", $"Role '{request.Rolename}' is edited.");
        }
        catch (ManualValidationException e) {
            return ErrorRedirect($"/modpanel/role/{roleId}/edit", e.Message);
        }
    }

    [HttpPost("/modpanel/role/{roleId:long}/delete"), Requires(Permissions.EDIT_ROLES)]
    public IActionResult DeleteRole(long roleId) {
        try {
            _editRoleService.DeleteRole(AuthContext.UserId, new RoleId(roleId));
            return SuccessRedirect("/modpanel/roles", "The role is deleted.");
        }
        catch (ManualValidationException e) {
            return ErrorRedirect("/modpanel/roles", e.Message);
        }
    }

    // --- Role users ---
    [HttpGet("/modpanel/role/{roleId:long}/users"), Requires(Permissions.EDIT_ROLES)]
    public IActionResult RoleUsers(long roleId) {
        try {
            var (role, authUsers) = _roleUserService.GetRoleWithUsers(new RoleId(roleId));
            return View(new RoleWithUsersViewModel(role, authUsers));
        }
        catch (ManualValidationException e) {
            return ErrorRedirect("/modpanel/roles", e.Message);
        }
    }

    [HttpPost("/modpanel/role/{roleId:long}/user"), Requires(Permissions.EDIT_ROLES)]
    public IActionResult AssignRoleToUser(long roleId, AssignUserToRoleRequest? request) {
        try {
            ValidateNotNull(request?.Username);

            _roleUserService.AssignRole(AuthContext.UserId, new RoleId(roleId), new Username(request!.Username));
            return SuccessRedirect($"/modpanel/role/{roleId}/users", "Users are assigned to the role.");
        }
        catch (ManualValidationException e) {
            return ErrorRedirect($"/modpanel/role/{roleId}/users", e.Message);
        }
    }

    [HttpPost("/modpanel/role/{roleId:long}/user/{userId:long}/remove"), Requires(Permissions.EDIT_ROLES)]
    public IActionResult RemoveRoleFromUser(long roleId, long userId) {
        try {
            _roleUserService.RemoveRole(AuthContext.UserId, new RoleId(roleId), new UserId(userId));
            return SuccessRedirect($"/modpanel/role/{roleId}/users", "The user is removed from this role.");
        }
        catch (ManualValidationException e) {
            return ErrorRedirect($"/modpanel/role/{roleId}/users", e.Message);
        }
    }

    // --- Admin logs ---
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

    private MenuViewModel BuildMenu() {
        return new MenuViewModel("Mod panel",
            ("Overview", "/modpanel"),
            ("Game tools", "/modpanel/game-tools"),
            ("Manage roles", "/modpanel/roles")
        );
    }

    public record ChangeTurnTimeRequest(long? GameId, int? TurnTimeInSeconds, bool? ForAllFutureTurns);
    public record EditRoleRequest(string? Rolename, string[]? Permissions);
    public record CreateRoleRequest(string? Rolename);
    public record AssignUserToRoleRequest(string? Username);
}
