using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Draughts.Controllers.Middleware;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Common {
    [ServiceFilter(typeof(JwtActionFilter))]
    [ServiceFilter(typeof(AuthContextActionFilter))]
    public class BaseController : Controller {
        private AuthContext? _authContext;
        private readonly List<(string field, string error)> _errors;

        public BaseController() {
            _errors = new List<(string field, string error)>();

            UpdateViewBag();
        }

        public bool IsLoggedIn => HttpContext != null && AuthContext.IsLoggedIn(HttpContext);
        public AuthContext AuthContext => _authContext ??= AuthContext.GetFromHttpContext(HttpContext);
        public AuthContext? AuthContextOrNull => _authContext ??= AuthContext.GetFromHttpContextOrNull(HttpContext);

        public IReadOnlyList<(string field, string error)> Errors => _errors.AsReadOnly();

        public void AddError(string error) => AddError("", error);
        public void AddError(string field, string error) => AddErrors(new[] { (field, error) });

        public void AddErrors(IEnumerable<string> errors) => AddErrors(errors.Select(e => ("", e)));
        public void AddErrors(IEnumerable<(string field, string error)> errors) {
            _errors.AddRange(errors);
            UpdateViewBag();
        }

        public ViewResult ErrorView(string error) => ErrorView("", error);
        public ViewResult ErrorView(string field, string error) {
            AddError(field, error);
            return View();
        }

        public ViewResult ErrorView(IEnumerable<string> errors) => ErrorView(errors.Select(e => ("", e)));
        public ViewResult ErrorView(IEnumerable<(string field, string error)> errors) {
            AddErrors(errors);
            return View();
        }

        public ViewResult ErrorRedirect(string url, string error) => ErrorRedirect(url, "", error);
        public ViewResult ErrorRedirect(string url, string field, string error) {
            if (url == "/") {
                url = "/StaticPages/Home";
            }
            AddError(field, error);
            return View($"~/Views{url}.cshtml");
        }

        public ViewResult ErrorRedirect(string url, IEnumerable<string> errors) => ErrorRedirect(url, errors.Select(e => ("", e)));
        public ViewResult ErrorRedirect(string url, IEnumerable<(string field, string error)> errors) {
            if (url == "/") {
                url = "/StaticPages/Home";
            }
            AddErrors(errors);
            return View($"~/Views{url}.cshtml");
        }

        public void UpdateViewBag() {
            ViewBag.IsLoggedIn = IsLoggedIn;
            ViewBag.AuthUserId = AuthContextOrNull?.AuthUserId;
            ViewBag.UserId = AuthContextOrNull?.UserId;
            ViewBag.Username = AuthContextOrNull?.Username;
            ViewBag.Permissions = AuthContextOrNull?.Permissions ?? new List<Permission>(0).AsReadOnly();

            ViewBag.Errors = Errors;
        }
    }
}
