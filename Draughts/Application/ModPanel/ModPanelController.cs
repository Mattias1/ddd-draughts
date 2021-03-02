using Draughts.Application.ModPanel.Services;
using Draughts.Application.ModPanel.ViewModels;
using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Draughts.Common;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;

namespace Draughts.Application.ModPanel {
    public class ModPanelController : BaseController {
        private readonly IModpanelRoleService _modpanelRoleService;
        private readonly IAdminLogRepository _adminLogRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ModPanelController(IAdminLogRepository adminLogRepository, IModpanelRoleService modpanelRoleService,
                IUnitOfWork unitOfWork) {
            _adminLogRepository = adminLogRepository;
            _modpanelRoleService = modpanelRoleService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("/modpanel"), Requires(Permissions.VIEW_MOD_PANEL)]
        public IActionResult ModPanel() {
            var adminLogs = GetAdminLogs();
            return View(new ModpanelOverviewViewModel(adminLogs, BuildMenu()));
        }

        [HttpGet("/modpanel/game-tools"), Requires(Permissions.EDIT_GAMES)]
        public IActionResult GameTools() {
            return View(new ModpanelViewModel(BuildMenu()));
        }

        [HttpGet("/modpanel/roles"), Requires(Permissions.EDIT_ROLES)]
        public IActionResult ManageRoles() {
            var roles = _modpanelRoleService.GetRoles();
            return View(new ModpanelRolesViewModel(roles, BuildMenu()));
        }

        [HttpGet("/modpanel/role/{roleId:long}/edit"), Requires(Permissions.EDIT_ROLES)]
        public IActionResult EditRole(long roleId) {
            try {
                var role = _modpanelRoleService.GetRole(new RoleId(roleId));
                return View(new RoleViewModel(role));
            }
            catch (ManualValidationException e) {
                return ErrorRedirect("/modpanel/roles", e.Message);
            }
        }

        [HttpPost("/modpanel/role/{roleId:long}/edit"), Requires(Permissions.EDIT_ROLES)]
        public IActionResult EditRolePost(long roleId, [FromForm]EditRoleRequest? request) {
            try {
                ValidateNotNull(request?.Rolename, request?.Permissions);

                _modpanelRoleService.EditRole(AuthContext.UserId, new RoleId(roleId),
                    request!.Rolename!, request.Permissions!);

                return Redirect("/modpanel/roles"); // TODO: Success message
            }
            catch (ManualValidationException e) {
                return ErrorRedirect($"/modpanel/role/{roleId}/edit", e.Message);
            }
        }

        // TODO: New role

        [HttpGet("/modpanel/role/{roleId:long}/users"), Requires(Permissions.EDIT_ROLES)]
        public IActionResult RoleUsers(long roleId) {
            // TODO
            return ErrorRedirect("/modpanel/roles", "TODO role users");
        }

        [HttpPost("/modpanel/role/{roleId:long}/delete"), Requires(Permissions.EDIT_ROLES)]
        public IActionResult DeleteRole(long roleId) {
            // TODO
            return ErrorRedirect("/modpanel/roles", "TODO delete role");
        }

        [HttpGet("/modpanel/admin-logs"), Requires(Permissions.VIEW_ADMIN_LOGS)]
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
                ("Overview", "/modpanel"),
                ("Game tools", "/modpanel/game-tools"),
                ("Manage roles", "/modpanel/roles")
            );
        }

        public record EditRoleRequest(string? Rolename, string[]? Permissions);
    }
}
