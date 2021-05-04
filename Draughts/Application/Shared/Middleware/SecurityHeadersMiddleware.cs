using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Draughts.Application.Shared.Middleware {
    public class SecurityHeadersMiddleware {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {        
            IHeaderDictionary headers = context.Response.Headers;

            string csp = "default-src 'none'; "
                + " script-src 'self'; connect-src 'self'; "
                + "img-src 'self'; style-src 'self' 'unsafe-inline'";
            headers["Content-Security-Policy"] = csp;
            headers["X-Content-Type-Options"] = "nosniff";
            headers["Referrer-Policy"] = "no-referrer";

            // Since this is not live on https, let's not set the HSTS or the 'upgrade-insecure-requests' part of the CSP.

            await _next(context);
        }
    }
}
