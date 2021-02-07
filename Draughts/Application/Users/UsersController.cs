using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Domain.UserAggregate.Specifications;
using Draughts.Repositories;
using Draughts.Repositories.Transaction;
using Microsoft.AspNetCore.Mvc;

namespace Draughts.Application.Users {
    public class UsersController : BaseController {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public UsersController(IUnitOfWork unitOfWork, IUserRepository userRepository) {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        [HttpGet("/user/{userId:long}"), GuestRoute]
        public IActionResult Userprofile(long userId) {
            var user = _unitOfWork.WithUserTransaction(tran => {
                var user = _userRepository.FindByIdOrNull(new UserId(userId));
                return tran.CommitWith(user);
            });

            if (user is null) {
                return NotFound();
            }

            return View(new UserViewModel(user));
        }

        [HttpGet("/user/list"), GuestRoute]
        public IActionResult Userlist() {
            var users = _unitOfWork.WithUserTransaction(tran => {
                var users = _userRepository.List(new RankSort());
                return tran.CommitWith(users);
            });

            return View(new UserlistViewModel(users));
        }
    }
}
