using Microsoft.AspNetCore.Http;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Draughts.Application.Shared.Middleware;

public sealed class SecurityHeadersMiddleware {
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next) {
        _next = next;
    }

    public async Task Invoke(HttpContext context) {
        IHeaderDictionary headers = context.Response.Headers;

        string nonce = GenerateNonce(context);
        string csp = "upgrade-insecure-requests; "
            + "default-src 'none'; "
            + $" script-src 'self' 'nonce-{nonce}'; connect-src 'self'; "
            + "img-src 'self'; style-src 'self' 'unsafe-inline'";

        headers["Content-Security-Policy"] = csp;
        headers["X-Content-Type-Options"] = "nosniff";
        headers["Referrer-Policy"] = "no-referrer";

        await _next(context);
    }

    private static string GenerateNonce(HttpContext context) {
        string nonce = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
        WebsiteContext.AttachToHttpContext(context, nonce);
        return nonce;
    }
}
