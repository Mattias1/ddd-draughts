using Draughts.Application.Shared;
using Draughts.Application.Shared.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Draughts.Application.StaticPages {
    public class StaticPagesController : BaseController {
        private readonly ILogger<StaticPagesController> _logger;

        public StaticPagesController(ILogger<StaticPagesController> logger) => _logger = logger;

        [HttpGet, GuestRoute]
        public IActionResult Home() => View();

        [HttpGet("/privacy"), GuestRoute]
        public IActionResult Privacy() => View();

        [HttpGet("/license"), GuestRoute]
        public IActionResult License() => View();

        [HttpGet("/error"), GuestRoute]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(HttpStatusCode? status) {
            ViewBag.StatusString = status?.ToString() ?? "Error";
            ViewBag.StatusCode = (int)(status ?? HttpStatusCode.InternalServerError);

            return View();
        }
    }
}
