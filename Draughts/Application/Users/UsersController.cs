using Microsoft.AspNetCore.Mvc;
using Draughts.Repositories;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Domain.UserAggregate.Specifications;
using Draughts.Common;
using Draughts.Application.Shared.ViewModels;
using Draughts.Application.Shared.Attributes;

namespace Draughts.Application.Users {
    public class UsersController : BaseController {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository) => _userRepository = userRepository;

        [HttpGet("/user/{userId:long}"), GuestRoute]
        public IActionResult Userprofile(long userId) {
            var user = _userRepository.FindByIdOrNull(new UserId(userId));
            if (user is null) {
                Userlist(); // This call is needed to set the view model :(
                return ErrorRedirect("/user/list", $"User not found with id {userId}.");
            }

            return View(new UserViewModel(user));
        }

        [HttpGet("/user/list"), GuestRoute]
        public IActionResult Userlist() {
            var users = _userRepository.List(new RankSort());

            return View(new UserlistViewModel(users));
        }
    }
}
