using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Draughts.Application.Documentation;

public class DocumentationController : BaseController {
    [HttpGet("/docs"), GuestRoute]
    public IActionResult DocsShortcut() => RedirectPermanent("/documentation");

    [HttpGet("/documentation"), GuestRoute]
    public IActionResult DocumentationOverview() => ViewWithMenu();

    [HttpGet, GuestRoute]
    public IActionResult DesignPrinciples() => ViewWithMenu();

    [HttpGet, GuestRoute]
    public IActionResult BoundedContexts() => ViewWithMenu();

    [HttpGet, GuestRoute]
    public IActionResult HexagonalArchitecture() => ViewWithMenu();

    [HttpGet, GuestRoute]
    public IActionResult BuildingBlocks() => ViewWithMenu();

    [HttpGet, GuestRoute]
    public IActionResult GoodToKnow() => ViewWithMenu();

    [HttpGet, GuestRoute]
    public IActionResult Auth() => ViewWithMenu();

    private IActionResult ViewWithMenu() {
        return View(new MenuViewModel("Subpages",
            ("Overview", "/documentation"),
            ("Design principles", "/documentation/designprinciples"),
            ("Bounded contexts", "/documentation/boundedcontexts"),
            ("Hexagonal architecture", "/documentation/hexagonalarchitecture"),
            ("Building blocks", "/documentation/buildingblocks"),
            ("Good to know", "/documentation/goodtoknow"),
            ("Authentication and authorization", "/documentation/auth")
        ));
    }
}
