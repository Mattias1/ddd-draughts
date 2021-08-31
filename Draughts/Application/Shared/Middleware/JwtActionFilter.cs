using Draughts.Common;
using Draughts.Common.Utilities;
using Draughts.Application.Auth.Services;
using Draughts.Application.Shared.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NodaTime;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using Draughts.Repositories.Transaction;

namespace Draughts.Application.Shared.Middleware {
    public class JwtActionFilter : IAuthorizationFilter {
        private readonly IClock _clock;
        private readonly AuthService _authService;
        private readonly IUnitOfWork _unitOfWork;

        public JwtActionFilter(IClock clock, AuthService authService, IUnitOfWork unitOfWork) {
            _clock = clock;
            _authService = authService;
            _unitOfWork = unitOfWork;
        }

        public void OnAuthorization(AuthorizationFilterContext context) {
            bool hasGuestAttribute = context.Filters.Any(f => f is GuestRouteAttribute);
            var requiredPermissions = context.Filters.OfType<RequiresAttribute>().SelectMany(r => r.Permissions).ToList();

            bool hasJwt = TryGetJwtFromCookieOrHeader(context.HttpContext, out var jwt);
            if (!hasJwt && !hasGuestAttribute) {
                FailRequest(context, HttpStatusCode.Unauthorized);
                return;
            }

            if (!hasJwt && hasGuestAttribute) {
                return;
            }

            var userPermissions = _unitOfWork.WithAuthTransaction(tran => {
                return _authService.PermissionsForJwt(jwt!);
            });
            AuthContext.AttachToHttpContext(jwt!, userPermissions, context.HttpContext);

            if (!hasGuestAttribute) {
                if (requiredPermissions.Count == 0) {
                    throw new InvalidOperationException("No permission requirements found, but it's not a guest route. Whut?");
                }

                if (!userPermissions.ContainsAll(requiredPermissions)) {
                    FailRequest(context, HttpStatusCode.Forbidden);
                    return;
                }
            }
        }

        private bool TryGetJwtFromCookieOrHeader(HttpContext httpContext, [NotNullWhen(returnValue: true)] out JsonWebToken? jwt) {
            if (!httpContext.Request.Cookies.TryGetValue(AuthContext.AUTHORIZATION_HEADER, out string? bearerToken)) {
                if (!httpContext.Request.Headers.TryGetValue(AuthContext.AUTHORIZATION_HEADER, out var authHeaderValue)) {
                    jwt = null;
                    return false;
                }
                bearerToken = authHeaderValue.ToString();
            }

            if (bearerToken is null || !bearerToken.StartsWith(AuthContext.BEARER_PREFIX)) {
                jwt = null;
                return false;
            }

            return JsonWebToken.TryParseFromJwtString(bearerToken.Substring(AuthContext.BEARER_PREFIX.Length), _clock, out jwt);
        }

        private void FailRequest(AuthorizationFilterContext context, HttpStatusCode statusCode) {
            if (IsAjaxRequest(context.HttpContext)) {
                context.Result = new StatusCodeResult((int)statusCode);
            }
            else {
                context.Result = new RedirectResult($"/Error?status={statusCode}");
            }
        }

        private static bool IsAjaxRequest(HttpContext httpContext) {
            return httpContext.Request.Headers.TryGetValue("X-Requested-With", out var xRequestedWith)
                && xRequestedWith == "XMLHttpRequest";
        }
    }
}
