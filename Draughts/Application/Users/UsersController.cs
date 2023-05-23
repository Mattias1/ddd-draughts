using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Draughts.Common;
using Draughts.Domain.AuthContext.Models;
using Draughts.Domain.UserContext.Models;
using Draughts.Domain.UserContext.Specifications;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using Microsoft.AspNetCore.Mvc;

namespace Draughts.Application.Users;

public sealed class UsersController : BaseController {
    private const int PAGE_SIZE = 10;

    private readonly AuthUserRepository _authUserRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserRepository _userRepository;

    public UsersController(AuthUserRepository authUserRepository, IUnitOfWork unitOfWork,
            UserRepository userRepository) {
        _authUserRepository = authUserRepository;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
    }

    [HttpGet("/user/{userId:long}"), GuestRoute]
    public IActionResult Userprofile(long userId) {
        var user = _unitOfWork.WithUserTransaction(tran => {
            return _userRepository.FindByIdOrNull(new UserId(userId));
        });

        if (user is null) {
            return NotFound();
        }

        return View(new UserViewModel(user));
    }

    [HttpGet("/user/list"), GuestRoute]
    public IActionResult Userlist(int page = 1) {
        var users = _unitOfWork.WithUserTransaction(tran => {
            return _userRepository.Paginate(page, PAGE_SIZE, new RankSort());
        });

        return View(new UserlistViewModel(users));
    }

    [HttpGet("/user/account"), Requires(Permission.Permissions.PLAY_GAME)]
    public IActionResult AccountSettings() {
        var authUser = _unitOfWork.WithAuthTransaction(tran => {
            return _authUserRepository.FindById(AuthContext.UserId);
        });

        return View(new AuthUserViewModel(authUser));
    }

    [HttpPost("/user/account"), Requires(Permission.Permissions.PLAY_GAME)]
    public IActionResult EditAccountSettings([FromForm] AccountSettingsRequest? request) {
        try {
            // This is not domain logic, but let's-help-the-user logic. It's fine in here.
            if (request?.Password != request?.PasswordConfirm) {
                throw new ManualValidationException("PasswordConfirm", "The passwords do not match.");
            }
            _unitOfWork.WithAuthTransaction(tran => {
                var authUser = _authUserRepository.FindById(AuthContext.UserId);
                authUser.UpdateEmailOrPassword(request?.CurrentPassword, request?.Email, request?.Password);
                _authUserRepository.Save(authUser);
            });
            return SuccessRedirect("/user/account", "Account info updated successfully.");
        }
        catch (ManualValidationException e) {
            return ErrorRedirect("/user/account", e.Message);
        }
    }

    public record AccountSettingsRequest(string? Email, string? Password, string? PasswordConfirm,
        string? CurrentPassword);
}
