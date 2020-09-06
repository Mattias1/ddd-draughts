using Draughts.Common;
using Draughts.Controllers.Shared.Attributes;
using Draughts.Controllers.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;

namespace Draughts.Controllers {
    [Requires(Permissions.PLAY_GAME)]
    public class GamelistController : BaseController {
        [HttpGet]
        public IActionResult Pending() {
            return ViewWithMenu();
        }

        [HttpGet]
        public IActionResult Active() {
            return ViewWithMenu();
        }

        [HttpGet]
        public IActionResult Finished() {
            return ViewWithMenu();
        }

        private IActionResult ViewWithMenu() {
            return View(new MenuViewModel("Your games",
                ("Pending games", "/gamelist/pending"),
                ("Active games", "/gamelist/active"),
                ("Finished games", "/gamelist/finished"),
                ("Create a new game", "/lobby/create")
            ));
        }
    }
}
