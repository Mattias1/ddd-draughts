using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Draughts.Application.Shared.Attributes;

/// <summary>
/// This attribute allows controllers to adjust the view directory.
/// This is needed when a controller does not have the same name as the directory it is in
/// and ASP.NET won't know where to find its views.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ViewsFromAttribute : ActionFilterAttribute {
    public string Directory { get; }

    public ViewsFromAttribute(string directory) {
        Directory = directory;
    }
}
