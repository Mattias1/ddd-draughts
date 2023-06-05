using Microsoft.AspNetCore.Http;
using System;

namespace Draughts.Application.Shared.Middleware;

public sealed class WebsiteContext {
    private const string WEBSITE_CONTEXT = "websiteContext";
    private const string ERROR_NO_WEBSITE_CONTEXT = "Failed getting the website context.";

    public string Nonce { get; }

    private WebsiteContext(string nonce) {
        Nonce = nonce;
    }

    public static WebsiteContext AttachToHttpContext(HttpContext httpContext, string nonce) {
        var context = new WebsiteContext(nonce);
        httpContext.Items[WEBSITE_CONTEXT] = context;
        return context;
    }

    public static void Clear(HttpContext httpContext) {
        httpContext.Items.Remove(WEBSITE_CONTEXT);
    }

    public static WebsiteContext GetFromHttpContextOrThrow(HttpContext httpContext) {
        return GetFromHttpContextOrNull(httpContext) ?? throw new InvalidOperationException(ERROR_NO_WEBSITE_CONTEXT);
    }
    public static WebsiteContext? GetFromHttpContextOrNull(HttpContext? httpContext) {
        if (httpContext is not null && httpContext.Items.TryGetValue(WEBSITE_CONTEXT, out var context)) {
            return context as WebsiteContext;
        }
        return null;
    }
}
