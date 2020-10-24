using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Draughts.Application.Shared;

namespace Draughts.Application.Documentation {
    public class DocumentationController : BaseController {
        [HttpGet("/documentation"), GuestRoute]
        public IActionResult DocumentationOverview() => ViewWithMenu();

        [HttpGet, GuestRoute]
        public IActionResult DesignPrinciples() => ViewWithMenu();

        [HttpGet, GuestRoute]
        public IActionResult BoundedContexts() => ViewWithMenu();

        [HttpGet, GuestRoute]
        public IActionResult OnionArchitecture() => ViewWithMenu();

        [HttpGet, GuestRoute]
        public IActionResult BuildingBlocks() => ViewWithMenu();

        [HttpGet, GuestRoute]
        public IActionResult Auth() => ViewWithMenu();

        private IActionResult ViewWithMenu() {
            return View(new MenuViewModel("Subpages",
                ("Overview", "/documentation"),
                ("Design principles", "/documentation/designprinciples"),
                ("Bounded contexts", "/documentation/boundedcontexts"),
                ("Onion architecture", "/documentation/onionarchitecture"),
                ("Building blocks", "/documentation/buildingblocks"),
                ("Authentication and authorization", "/documentation/auth")
            ));
        }
    }
}
