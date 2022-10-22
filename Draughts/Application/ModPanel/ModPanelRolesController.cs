using Draughts.Application.Auth.Services;
using Draughts.Application.ModPanel.ViewModels;
using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Common;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.AuthContext.Specifications;
using Draughts.Domain.UserContext.Models;
using Draughts.Repositories;
using Draughts.Repositories.Misc;
using Draughts.Repositories.Transaction;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using System.Linq;
using static Draughts.Domain.AuthContext.Models.Permission;

namespace Draughts.Application.ModPanel;

[ViewsFrom("ModPanel")]
public sealed class ModPanelRolesController : BaseController {
    private readonly AdminLogFactory _adminLogFactory;
    private readonly AuthUserRepository _authUserRepository;
    private readonly IClock _clock;
    private readonly IIdGenerator _idGenerator;
    private readonly RoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ModPanelRolesController(AdminLogFactory adminLogFactory, AuthUserRepository authUserRepository,
            IClock clock, IIdGenerator idGenerator, RoleRepository roleRepository, IUnitOfWork unitOfWork) {
        _adminLogFactory = adminLogFactory;
        _authUserRepository = authUserRepository;
        _clock = clock;
        _idGenerator = idGenerator;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }

    // --- Manage roles ---
    [HttpGet("/modpanel/roles"), Requires(Permissions.EDIT_ROLES)]
    public IActionResult ManageRoles() {
        var roles = _unitOfWork.WithAuthTransaction(tran => {
            return _roleRepository.List();
        });
        return View(new ModPanelRolesViewModel(roles, ModPanelController.BuildMenu()));
    }

    // --- Edit role ---
    [HttpGet("/modpanel/role/{rawRoleId:long}/edit"), Requires(Permissions.EDIT_ROLES)]
    public IActionResult EditRole(long rawRoleId) {
        try {
            var role = _unitOfWork.WithAuthTransaction(tran => {
                return FindRole(new RoleId(rawRoleId));
            });
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

            var role = _unitOfWork.WithAuthTransaction(tran => {
                var role = Role.CreateNew(_idGenerator.ReservePool(), request!.Rolename!, _clock);
                _adminLogFactory.LogCreateRole(AuthContext.UserId, AuthContext.Username, role.Id, role.Rolename);
                _roleRepository.Save(role);

                return role;
            });

            return SuccessRedirect($"/modpanel/role/{role.Id}/edit", $"Role '{role.Rolename}' is added.");
        }
        catch (ManualValidationException e) {
            return ErrorRedirect($"/modpanel/roles", e.Message);
        }
    }

    [HttpPost("/modpanel/role/{rawRoleId:long}/edit"), Requires(Permissions.EDIT_ROLES)]
    public IActionResult EditRolePost(long rawRoleId, [FromForm] EditRoleRequest? request) {
        try {
            ValidateNotNull(request?.Rolename, request?.Permissions);

            _unitOfWork.WithAuthTransaction(tran => {
                var role = FindRole(new RoleId(rawRoleId));
                _adminLogFactory.LogEditRole(AuthContext.UserId, AuthContext.Username, role.Id, role.Rolename);
                role.Edit(request!.Rolename!, request!.Permissions!.Select(p => new Permission(p)));

                _roleRepository.Save(role);
            });

            return SuccessRedirect("/modpanel/roles", $"Role '{request?.Rolename}' is edited.");
        }
        catch (ManualValidationException e) {
            return ErrorRedirect($"/modpanel/role/{rawRoleId}/edit", e.Message);
        }
    }

    [HttpPost("/modpanel/role/{rawRoleId:long}/delete"), Requires(Permissions.EDIT_ROLES)]
    public IActionResult DeleteRole(long rawRoleId) {
        try {
            _unitOfWork.WithAuthTransaction(tran => {
                var role = FindRole(new RoleId(rawRoleId));
                long nrOfUsersWithRole = _authUserRepository.Count(new UsersWithRoleSpecification(role.Id));
                if (nrOfUsersWithRole > 0) {
                    throw new ManualValidationException("You cannot delete roles with users assigned.");
                }

                _adminLogFactory.LogDeleteRole(AuthContext.UserId, AuthContext.Username, role.Id, role.Rolename);
                _roleRepository.Delete(role.Id);
            });
            return SuccessRedirect("/modpanel/roles", "The role is deleted.");
        }
        catch (ManualValidationException e) {
            return ErrorRedirect("/modpanel/roles", e.Message);
        }
    }

    // --- Role users ---
    [HttpGet("/modpanel/role/{rawRoleId:long}/users"), Requires(Permissions.EDIT_ROLES)]
    public IActionResult RoleUsers(long rawRoleId) {
        try {
            var roleId = new RoleId(rawRoleId);
            var (role, authUsers) = _unitOfWork.WithAuthTransaction(tran => {
                var role = FindRole(roleId);
                var authUsers = _authUserRepository.List(new UsersWithRoleSpecification(roleId));
                return (role, authUsers);
            });
            return View(new RoleWithUsersViewModel(role, authUsers));
        }
        catch (ManualValidationException e) {
            return ErrorRedirect("/modpanel/roles", e.Message);
        }
    }

    [HttpPost("/modpanel/role/{rawRoleId:long}/user"), Requires(Permissions.EDIT_ROLES)]
    public IActionResult AssignRoleToUser(long rawRoleId, AssignUserToRoleRequest? request) {
        try {
            ValidateNotNull(request?.Username);

            _unitOfWork.WithAuthTransaction(tran => {
                var role = FindRole(new RoleId(rawRoleId));
                var authUser = _authUserRepository.FindOrNull(new UsernameSpecification(new Username(request!.Username)));
                if (authUser is null) {
                    throw new ManualValidationException("User not found");
                }

                _adminLogFactory.LogGainRole(AuthContext.UserId, AuthContext.Username,
                    role.Id, role.Rolename, authUser.Id, authUser.Username);
                authUser.AssignRole(role.Id, role.Rolename);
                _authUserRepository.Save(authUser);
            });
            return SuccessRedirect($"/modpanel/role/{rawRoleId}/users", "Users are assigned to the role.");
        }
        catch (ManualValidationException e) {
            return ErrorRedirect($"/modpanel/role/{rawRoleId}/users", e.Message);
        }
    }

    [HttpPost("/modpanel/role/{rawRoleId:long}/user/{rawUserId:long}/remove"), Requires(Permissions.EDIT_ROLES)]
    public IActionResult RemoveRoleFromUser(long rawRoleId, long rawUserId) {
        try {
            var roleId = new RoleId(rawRoleId);
            var userId = new UserId(rawUserId);
            _unitOfWork.WithAuthTransaction(tran => {
                var role = FindRole(roleId);
                var authUser = _authUserRepository.FindByIdOrNull(userId);
                if (authUser is null) {
                    throw new ManualValidationException("User not found");
                }

                _adminLogFactory.LogLoseRole(AuthContext.UserId, AuthContext.Username,
                    role.Id, role.Rolename, authUser.Id, authUser.Username);
                authUser.RemoveRole(role.Id, role.Rolename);
                _authUserRepository.Save(authUser);
            });
            return SuccessRedirect($"/modpanel/role/{rawRoleId}/users", "The user is removed from this role.");
        }
        catch (ManualValidationException e) {
            return ErrorRedirect($"/modpanel/role/{rawRoleId}/users", e.Message);
        }
    }

    private Role FindRole(RoleId roleId) {
        var role = _roleRepository.FindByIdOrNull(roleId);
        if (role is null) {
            throw new ManualValidationException("Role not found");
        }
        return role;
    }

    public record EditRoleRequest(string? Rolename, string[]? Permissions);
    public record CreateRoleRequest(string? Rolename);
    public record AssignUserToRoleRequest(string? Username);
}
