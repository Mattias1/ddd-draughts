using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Draughts.Application.Shared.Attributes {
    /// <summary>
    /// Identifies an acion that can be visited by guests, and does not require authorization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)] // Do not allow class usage; the default should be block, not allow.
    public class GuestRouteAttribute : ActionFilterAttribute { }
}
