using Draughts.Common;
using Draughts.Domain.AuthUserAggregate.Models;
using Draughts.Domain.UserAggregate.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace Draughts.Application.Shared.Middleware {
    public class AuthContext {
        public const string AUTHORIZATION_HEADER = "Authorization";
        public const string BEARER_PREFIX = "Bearer ";

        private const string AUTH_CONTEXT = "authContext";
        private const string ERROR_NO_AUTH = "Trying to get the auth context when not logged in.";

        public AuthUserId AuthUserId { get; }
        public UserId UserId { get; }
        public Username Username { get; }
        public IReadOnlyList<Permission> Permissions { get; }

        private AuthContext(JsonWebToken jwt, IReadOnlyList<Permission> permissions) {
            AuthUserId = jwt.AuthUserId;
            UserId = jwt.UserId;
            Username = jwt.Username;
            Permissions = permissions;
        }

        private void UpdateHttpContext(JsonWebToken jwt, HttpContext httpContext) {
            httpContext.Items[AUTH_CONTEXT] = this;
            httpContext.Response.Cookies.Append(AUTHORIZATION_HEADER, BEARER_PREFIX + jwt.ToJwtString(), new CookieOptions {
                HttpOnly = true,
                IsEssential = true,
                MaxAge = TimeSpan.FromSeconds(JsonWebToken.EXPIRATION_SECONDS),
                SameSite = SameSiteMode.Strict,
                Secure = true
            });
        }

        public void Clear(HttpContext httpContext) {
            httpContext.Items.Remove(AUTH_CONTEXT);
            httpContext.Response.Cookies.Delete(AUTHORIZATION_HEADER);
        }

        public static AuthContext AttachToHttpContext(JsonWebToken jwt, IReadOnlyList<Permission> permissions, HttpContext httpContext) {
            var authContext = new AuthContext(jwt, permissions);
            authContext.UpdateHttpContext(jwt, httpContext);

            return authContext;
        }

        public static bool IsLoggedIn(HttpContext httpContext) => httpContext.Items.ContainsKey(AUTH_CONTEXT);

        public static AuthContext GetFromHttpContext(HttpContext httpContext) {
            return GetFromHttpContextOrNull(httpContext) ?? throw new InvalidOperationException(ERROR_NO_AUTH);
        }
        public static AuthContext? GetFromHttpContextOrNull(HttpContext? httpContext) {
            if (httpContext is not null && httpContext.Items.TryGetValue(AUTH_CONTEXT, out var authContext)) {
                return authContext as AuthContext;
            }
            return null;
        }
    }
}
