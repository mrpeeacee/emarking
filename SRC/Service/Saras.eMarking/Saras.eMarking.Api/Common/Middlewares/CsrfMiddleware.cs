using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Saras.eMarking.Business;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Entities.Security;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Common.Middlewares
{
    public class CsrfMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAntiforgery antiforgery;
        private readonly AppOptions appOptions;
        private readonly JwtOptions jwtOptions;
        public CsrfMiddleware(RequestDelegate next, IAntiforgery antiforgery, AppOptions _appOptions)
        {
            appOptions = _appOptions;
            this.antiforgery = antiforgery;
            jwtOptions = _appOptions.JwtOptions;
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (appOptions.AppSettings.IsCsrfValidationEnabled)
            {
                if ((httpContext.Request.Method == HttpMethods.Post
                    || httpContext.Request.Method == HttpMethods.Patch
                    || httpContext.Request.Method == HttpMethods.Put
                    || httpContext.Request.Method == HttpMethods.Delete) && httpContext.Request.Cookies.ContainsKey("X-XSRF-TOKEN"))
                {
                    string csrftoken = httpContext.Request.Cookies["X-XSRF-TOKEN"].Trim();

                    if (!string.IsNullOrEmpty(csrftoken))
                    {
                        httpContext.Request.Headers.Append("X-XSRF-TOKEN", csrftoken);
                    }
                }
                var tokens = antiforgery.GetTokens(httpContext);
                Utilities.InsertStringToCookie(httpContext, "X-XSRF-TOKEN", tokens.RequestToken, jwtOptions.TokenValidityInMinutes);
                antiforgery.SetCookieTokenAndHeader(httpContext);
            }
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CsrfMiddlewareExtensions
    {
        public static IApplicationBuilder UseCsrfMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CsrfMiddleware>();
        }
    }
}
