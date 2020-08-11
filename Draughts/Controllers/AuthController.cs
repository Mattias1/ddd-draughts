using Microsoft.AspNetCore.Mvc;
using Draughts.Common;
using Draughts.Controllers.Middleware;
using Draughts.Controllers.Attributes;
using Draughts.Services;
using Draughts.Repositories.Databases;

namespace Draughts.Controllers {
    public class AuthController : BaseController {
        private const string ALREADY_LOGGED_IN_ERROR = "You're already logged in.";

        private readonly IAuthService _authService;
        private readonly IAuthUserFactory _authUserFactory;
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(IAuthService authService, IAuthUserFactory authUserFactory, IUnitOfWork unitOfWork) {
            _authService = authService;
            _authUserFactory = authUserFactory;
            _unitOfWork = unitOfWork;
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
            if (IsLoggedIn) {
                return ErrorRedirect("/", ALREADY_LOGGED_IN_ERROR);
            }

            try {
                // This is no domain logic, but let's-help-the-user logic. It's fine in here.
                if (request.Password != request.PasswordConfirm) {
                    throw new ManualValidationException("PasswordConfirm", "The passwords do not match.");
                }

                // Do I want this transaction in a controller? Probably not right? Because we might need transactions
                // for different domains. And they should be accessed from inside a non-domain service.
                // Although, we do always need events for that right? So this might just be fine.
                _unitOfWork.WithTransaction(TransactionDomain.AuthUser, tran => {
                    _authUserFactory.CreateAuthUser(request.Name, request.Email, request.Password);

                    tran.Commit();
                });
                return Redirect("/");
            }
            catch (ManualValidationException e) {
                return ErrorRedirect("/auth/register", e.Message);
            }
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
