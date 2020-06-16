using Microsoft.AspNetCore.Mvc;
using Draughts.Common;
using Draughts.Middleware;
using Draughts.Controllers.Attributes;
using Draughts.Controllers.Services;

namespace Draughts.Controllers {
    public class AuthController : BaseController {
        private const string ALREADY_LOGGED_IN_ERROR = "You're already logged in.";

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) {
            _authService = authService;
        }

        [HttpGet, GuestRoute]
        public IActionResult Login() {
            if (IsLoggedIn) {
                return ErrorRedirect("/", ALREADY_LOGGED_IN_ERROR);
            }
            return View();
        }

        [HttpPost, GuestRoute]
        public IActionResult Login([FromForm] LoginRequest request) {
            try {
                var jwt = _authService.GenerateJwt(request.Name, request.Password);
                var permissions = _authService.PermissionsForJwt(jwt);
                AuthContext.AttachToHttpContext(jwt, permissions, HttpContext);

                return Redirect("/");
            }
            catch (ManualValidationException) {
                return ErrorView("password", "Incorrect username or password.");
            }
        }

        [HttpPost, GuestRoute]
        public IActionResult Logout() {
            if (IsLoggedIn) {
                AuthContext.Clear(HttpContext);
            }

            return Redirect("/");
        }

        [HttpGet, GuestRoute]
        public IActionResult Register() {
            if (IsLoggedIn) {
                return ErrorRedirect("/", ALREADY_LOGGED_IN_ERROR);
            }
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
