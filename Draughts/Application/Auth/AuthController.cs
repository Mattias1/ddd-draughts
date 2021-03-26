using Microsoft.AspNetCore.Mvc;
using Draughts.Common;
using Draughts.Repositories.Transaction;
using Draughts.Application.Shared.Attributes;
using Draughts.Application.Shared.Middleware;
using Draughts.Application.Auth.Services;
using Draughts.Application.Shared;
using Draughts.Repositories;

namespace Draughts.Application.Auth {
    public class AuthController : BaseController {
        private const string ALREADY_LOGGED_IN_ERROR = "You're already logged in.";

        private readonly IAuthService _authService;
        private readonly IAuthUserFactory _authUserFactory;
        private readonly IIdGenerator _idGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(IAuthService authService, IAuthUserFactory authUserFactory, IIdGenerator idGenerator,
                IUnitOfWork unitOfWork) {
            _authService = authService;
            _authUserFactory = authUserFactory;
            _idGenerator = idGenerator;
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
        public IActionResult Login([FromForm] LoginRequest? request) {
            try {
                ValidateNotNull(request?.Name, request?.Password);

                var (jwt, permissions) = _unitOfWork.WithAuthUserTransaction(tran => {
                    var jwt = _authService.GenerateJwt(request!.Name, request.Password);
                    var permissions = _authService.PermissionsForJwt(jwt);

                    return tran.CommitWith((jwt, permissions));
                });
                AuthContext authContext = AuthContext.AttachToHttpContext(jwt, permissions, HttpContext);

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
        public IActionResult Register([FromForm] RegistrationRequest? request) {
            if (IsLoggedIn) {
                return ErrorRedirect("/", ALREADY_LOGGED_IN_ERROR);
            }

            try {
                ValidateNotNull(request?.Name, request?.Email, request?.Password, request?.PasswordConfirm);

                // This is no domain logic, but let's-help-the-user logic. It's fine in here.
                if (request!.Password != request.PasswordConfirm) {
                    throw new ManualValidationException("PasswordConfirm", "The passwords do not match.");
                }

                // Do I want this transaction in a controller? Probably not right? Because we might need transactions
                // (or at least read only db connections) for different domains. And they should be accessed from inside
                // a non-domain service.
                _unitOfWork.WithAuthUserTransaction(tran => {
                    _authUserFactory.CreateAuthUser(_idGenerator.ReservePool(), request.Name, request.Email, request.Password);

                    tran.Commit();
                });
                return SuccessRedirect("/", $"User '{request.Name}' is registered.");
            }
            catch (ManualValidationException e) {
                return ErrorRedirect("/auth/register", e.Message);
            }
        }

        public record LoginRequest(string? Name, string? Password);
        public record RegistrationRequest(string? Name, string? Email, string? Password, string? PasswordConfirm);
    }
}
