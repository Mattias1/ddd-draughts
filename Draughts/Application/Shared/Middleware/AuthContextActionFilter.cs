using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Draughts.Application.Shared.Middleware {
    public class AuthContextActionFilter : IActionFilter {
        public const string ERROR_PARAM = "error";
        public const char ERROR_SEPARATOR = '|';

        public void OnActionExecuting(ActionExecutingContext context) {
            if (context.Controller is BaseController controller) {
                if (context.HttpContext.Request.Query.TryGetValue(ERROR_PARAM, out var errorMessages)) {
                    controller.AddErrors(errorMessages.ToString().Split(ERROR_SEPARATOR, StringSplitOptions.RemoveEmptyEntries));
                }
                else {
                    controller.UpdateViewBag();
                }
            }
            else {
                throw new InvalidOperationException("We arrive at a controller that doesn't override the BaseController. How?");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
