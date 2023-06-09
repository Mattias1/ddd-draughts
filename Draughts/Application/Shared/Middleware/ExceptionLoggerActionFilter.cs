using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Draughts.Application.Shared.Middleware;

public sealed class ExceptionLoggerActionFilter : IExceptionFilter {
    private readonly ILogger<ExceptionLoggerActionFilter> _log;

    public ExceptionLoggerActionFilter(ILogger<ExceptionLoggerActionFilter> log) {
        _log = log;
    }

    public void OnException(ExceptionContext context) {
        _log.LogError(context.Exception, "Uncaught exception");
    }
}
