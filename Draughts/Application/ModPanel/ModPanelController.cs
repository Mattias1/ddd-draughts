using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Microsoft.AspNetCore.Mvc;
using static Draughts.Domain.AuthUserAggregate.Models.Permission;

namespace Draughts.Application.ModPanel {
    [Requires(Permissions.VIEW_MOD_PANEL)]
    public class ModPanelController : BaseController {
        [HttpGet("/modpanel")]
        public IActionResult ModPanel() {
            return View();
        }
    }
}
