using Draughts.Common;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Draughts.Application.Shared.Middleware {
    public class AuthContextActionFilter : IActionFilter {
        public void OnActionExecuting(ActionExecutingContext context) {
            if (context.Controller is BaseController controller) {
                controller.UpdateViewBag();
            }
            else {
                throw new InvalidOperationException("We arrive at a controller that doesn't override the BaseController. How?");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
