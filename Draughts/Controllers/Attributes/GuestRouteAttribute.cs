using Microsoft.AspNetCore.Mvc.Filters;

namespace Draughts.Controllers.Attributes {
    /// <summary>
    /// Identifies an acion that can be visited by guests, and does not require authorization.
    /// </summary>
    public class GuestRouteAttribute : ActionFilterAttribute { }
}
