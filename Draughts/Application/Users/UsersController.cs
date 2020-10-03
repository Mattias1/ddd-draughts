using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Domain.UserAggregate.Specifications;
using Draughts.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Draughts.Application.Users {
    public class UsersController : BaseController {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository) => _userRepository = userRepository;

        [HttpGet("/user/{userId:long}"), GuestRoute]
        public IActionResult Userprofile(long userId) {
            var user = _userRepository.FindByIdOrNull(new UserId(userId));

            if (user is null) {
                return NotFound();
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
