using Draughts.Common;
using Draughts.Controllers.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Draughts.Controllers {
    public class LobbyController : BaseController {
        [HttpGet, GuestRoute]
        public IActionResult Index() {
            return View();
        }
    }
}
