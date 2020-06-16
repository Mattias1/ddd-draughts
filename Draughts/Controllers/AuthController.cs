using Microsoft.AspNetCore.Mvc;
using Draughts.Common;
using Draughts.Controllers.Attributes;

namespace Draughts.Controllers {
    public class AuthController : BaseController {
        [HttpGet, GuestRoute]
        public IActionResult Login() {
            return View();
        }

        [HttpPost, GuestRoute]
        public IActionResult Login([FromForm] LoginRequest request) {
            return ErrorRedirect("/", "TODO");
        }

        [HttpPost, GuestRoute]
        public IActionResult Logout() {
            return ErrorRedirect("/", "TODO");
        }

        [HttpGet, GuestRoute]
        public IActionResult Register() {
            return View();
        }

        [HttpPost, GuestRoute]
        public IActionResult Register([FromForm] RegistrationRequest request) {
            return ErrorRedirect("/", "TODO");
        }

        public class LoginRequest {
            public string? Name { get; set; }
            public string? Password { get; set; }
        }

        public class RegistrationRequest {
            public string? Name { get; set; }
            public string? Email { get; set; }
            public string? Password { get; set; }
            public string? PasswordConfirm { get; set; }
        }
    }
}
