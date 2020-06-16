using Draughts.Domain.AuthUserAggregate.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Draughts.Controllers.Attributes {
    /// <summary>
    /// Marks the permissions that are required for this route.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class RequiresAttribute : ActionFilterAttribute {
        public IReadOnlyList<Permission> Permissions { get; }

        public RequiresAttribute(params string[] permissions) {
            Permissions = permissions.Select(s => new Permission(s)).ToArray();
            if (Permissions.Count == 0) {
                throw new InvalidOperationException("No permissions required in the RequiresAttribute. That's not how it works, silly.");
            }
        }
    }
}
