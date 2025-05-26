using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Saras.eMarking.Domain.Entities;
using System.Net;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Common.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class AntiXssMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly AppOptions appOptions;
        public AntiXssMiddleware(RequestDelegate next, AppOptions _appOptions)
        {
            _next = next;
            appOptions = _appOptions;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (appOptions.AppSettings.IsXssValidationEnabled)
            {
                var sanitised = WebUtility.HtmlEncode(httpContext.Request.Path.Value);

                if (httpContext.Request.Path.Value != sanitised)
                {
                    throw new BadHttpRequestException("Special characters are not allowed!");
                }
            }
            await _next.Invoke(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AntiXssMiddlewareExtensions
    {
        public static IApplicationBuilder UseAntiXssMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AntiXssMiddleware>();
        }
    }
}
