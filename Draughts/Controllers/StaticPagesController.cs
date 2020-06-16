using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Draughts.Controllers {
    public class StaticPagesController : Controller {
        private readonly ILogger<StaticPagesController> _logger;

        public StaticPagesController(ILogger<StaticPagesController> logger) => _logger = logger;

        [HttpGet]
        public IActionResult Home() => View();
    }
}
