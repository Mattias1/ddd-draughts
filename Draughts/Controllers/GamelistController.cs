using Draughts.Common;
using Draughts.Controllers.Attributes;
using Microsoft.AspNetCore.Mvc;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;

namespace Draughts.Controllers {
    [Requires(Permissions.PLAY_GAME)]
    public class GamelistController : BaseController {
        [HttpGet]
        public IActionResult Pending() {
            return View();
        }

        [HttpGet]
        public IActionResult Active() {
            return View();
        }

        [HttpGet]
        public IActionResult Finished() {
            return View();
        }
    }
}
