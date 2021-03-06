using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Draughts.Application.Shared.Middleware {
    public class ExceptionLoggerActionFilter : IExceptionFilter {
        private readonly ILogger _log;

        public ExceptionLoggerActionFilter(ILogger<ExceptionLoggerActionFilter> log) {
            _log = log;
        }

        public void OnException(ExceptionContext context) {
            _log.LogError("Uncaught exception", context.Exception);
        }
    }
}
