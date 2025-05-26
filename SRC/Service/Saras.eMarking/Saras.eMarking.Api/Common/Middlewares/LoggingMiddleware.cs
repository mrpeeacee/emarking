using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Common.Middlewares
{

    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public readonly ILogger logger;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> _logger)
        {
            _next = next;
            logger = _logger;
        }

        public Task Invoke(HttpContext httpContext)
        {
            string sessionid = httpContext.Request.Headers["x-header"];
            if (string.IsNullOrEmpty(sessionid))
            {
                sessionid = "LoginPage";
            }
            var loggerState = new Dictionary<string, object>
            {
                ["CorrelationID"] = sessionid
                //Add any number of properties to be logged under a single scope
            }; 
            using (logger.BeginScope<Dictionary<string, object>>(loggerState))
            {
                return _next(httpContext);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoggingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
