using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;
using Draughts.Application.Shared;

namespace Draughts.Application {
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
