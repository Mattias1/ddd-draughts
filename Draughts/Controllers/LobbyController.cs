using Draughts.Common;
using Draughts.Controllers.Attributes;
using Microsoft.AspNetCore.Mvc;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;

namespace Draughts.Controllers {
    public class LobbyController : BaseController {
        [HttpGet("/lobby"), GuestRoute]
        public IActionResult Lobby() {
            return View();
        }

        [HttpGet("lobby/create"), Requires(Permissions.PLAY_GAME)]
        public IActionResult Create() {
            return View();
        }

        [HttpPost("lobby/create"), Requires(Permissions.PLAY_GAME)]
        public IActionResult PostCreate() {
            return Redirect("/lobby");
        }
    }
}
