using Draughts.Application.Shared.Middleware;
using Draughts.Common;
using Draughts.Domain.AuthUserAggregate.Models;
using Flurl;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Application.Shared {
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

        public void ValidateNotNull(params object?[] parameters) {
            if (parameters.Any(p => p is null)) {
                throw new ManualValidationException("Parameter is null.");
            }
        }

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
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return View();
        }

        public ViewResult ErrorView(IEnumerable<string> errors) => ErrorView(errors.Select(e => ("", e)));
        public ViewResult ErrorView(IEnumerable<(string field, string error)> errors) {
            AddErrors(errors);
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            return View();
        }

        public IActionResult ErrorRedirect(Url url, IEnumerable<string> errors) {
            var errorMessages = string.Join(AuthContextActionFilter.ERROR_SEPARATOR, errors);
            return ErrorRedirect(url, errorMessages);
        }
        public IActionResult ErrorRedirect(Url url, string error) {
            if (url.QueryParams.ContainsKey(AuthContextActionFilter.ERROR_PARAM)) {
                throw new InvalidOperationException($"You shouldn't set the {AuthContextActionFilter.ERROR_PARAM} param manually.");
            }
            url.SetQueryParam(AuthContextActionFilter.ERROR_PARAM, error);
            return Redirect(url);
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
