using Draughts.Common;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Draughts.Application.Documentation {
    public class DocumentationController : BaseController {
        [HttpGet("/documentation"), GuestRoute]
        public IActionResult Index() => ViewWithMenu();

        [HttpGet, GuestRoute]
        public IActionResult Auth() => ViewWithMenu();

        [HttpGet, GuestRoute]
        public IActionResult BoundedContexts() => ViewWithMenu();

        [HttpGet, GuestRoute]
        public IActionResult HexagonalArchitecture() => ViewWithMenu();

        private IActionResult ViewWithMenu() {
            return View(new MenuViewModel("Subpages",
                ("Overview", "/documentation"),
                ("Authentication and authorization", "/documentation/auth"),
                ("Bounded contexts", "/documentation/boundedcontexts"),
                ("Hexagonal architecture", "/documentation/hexagonalarchitecture")
            ));
        }
    }
}
