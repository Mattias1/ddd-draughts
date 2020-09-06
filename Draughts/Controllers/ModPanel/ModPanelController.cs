using Draughts.Common;
using Draughts.Controllers.Shared.Attributes;
using Microsoft.AspNetCore.Mvc;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;

namespace Draughts.Controllers {
    [Requires(Permissions.VIEW_MOD_PANEL)]
    public class ModPanelController : BaseController {
        [HttpGet("/modpanel")]
        public IActionResult Index() {
            return View();
        }
    }
}
