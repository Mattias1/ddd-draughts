using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Draughts.Application.Shared.Middleware;

public sealed class AuthContextActionFilter : IActionFilter {
    public const string SUCCESS_PARAM = "success";
    public const string ERROR_PARAM = "error";
    public const char ERROR_SEPARATOR = '|';

    public void OnActionExecuting(ActionExecutingContext context) {
        if (context.Controller is BaseController controller) {
            // Update the auth context
            controller.UpdateViewBag();

            // Pass the error and success messages to the controller
            // We use query parameters and not a session because this is a stateless application.
            if (context.HttpContext.Request.Query.TryGetValue(ERROR_PARAM, out var errorMessages)) {
                controller.AddErrors(errorMessages.ToString().Split(ERROR_SEPARATOR, StringSplitOptions.RemoveEmptyEntries));
            }
            if (context.HttpContext.Request.Query.TryGetValue(SUCCESS_PARAM, out var successMessage)) {
                controller.AddSuccessMessage(successMessage.ToString());
            }
        } else {
            throw new InvalidOperationException("We arrive at a controller that doesn't override the BaseController. How?");
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
