using Draughts.Application.ModPanel.Services;
using Draughts.Application.ModPanel.ViewModels;
using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Common;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using Microsoft.AspNetCore.Mvc;
using static Draughts.Domain.AuthContext.Models.Permission;

namespace Draughts.Application.ModPanel;

[ViewsFrom("ModPanel")]
public sealed class ModPanelRolesController : BaseController {
    private readonly EditRoleService _editRoleService;
    private readonly RoleUsersService _roleUserService;

    public ModPanelRolesController(EditRoleService editRoleService, RoleUsersService roleUsersService) {
        _editRoleService = editRoleService;
        _roleUserService = roleUsersService;
    }

    // --- Manage roles ---
    [HttpGet("/modpanel/roles"), Requires(Permissions.EDIT_ROLES)]
    public IActionResult ManageRoles() {
        var roles = _editRoleService.GetRoles();
        return View(new ModPanelRolesViewModel(roles, ModPanelController.BuildMenu()));
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

    public record EditRoleRequest(string? Rolename, string[]? Permissions);
    public record CreateRoleRequest(string? Rolename);
    public record AssignUserToRoleRequest(string? Username);
}
