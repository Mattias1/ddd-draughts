using Microsoft.AspNetCore.Mvc;
using Draughts.Repositories;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Domain.UserAggregate.Specifications;
using Draughts.Common;
using Draughts.Controllers.Attributes;
using Draughts.Controllers.Shared.ViewModels;

namespace Draughts.Controllers {
    public class UserController : BaseController {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository) => _userRepository = userRepository;

        [HttpGet("/user/{userId:long}"), GuestRoute]
        public IActionResult Index(long userId) {
            var user = _userRepository.FindByIdOrNull(new UserId(userId));
            if (user is null) {
                List(); // This call is needed to set the view model :(
                return ErrorRedirect("/user/list", $"User not found with id {userId}.");
            }

            return View(new UserViewModel(user));
        }

        [HttpGet, GuestRoute]
        public IActionResult List() {
            var users = _userRepository.List(new RankSort());

            return View(new UserlistViewModel(users));
        }
    }
}
