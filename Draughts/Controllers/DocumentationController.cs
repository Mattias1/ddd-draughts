using Draughts.Common;
using Draughts.Controllers.Attributes;
using Draughts.Controllers.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Draughts.Controllers {
    public class DocumentationController : BaseController {
        [HttpGet("/documentation"), GuestRoute]
        public IActionResult Index() => ViewWithMenu();

        [HttpGet, GuestRoute]
        public IActionResult BoundedContexts() => ViewWithMenu();

        private IActionResult ViewWithMenu() {
            return View(new MenuViewModel(
                ("Overview", "/documentation"),
                ("Bounded contexts", "/documentation/boundedcontexts")
            ));
        }
    }
}
