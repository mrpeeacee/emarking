using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Interfaces.BusinessInterface.Account;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Saras.eMarking.Api.Common.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        public readonly ILogger logger;
        public AuthMiddleware(RequestDelegate next, ILogger<AuthMiddleware> _logger)
        {
            _next = next;
            logger = _logger;
        }
        public async Task Invoke(HttpContext httpContext, IAuthService _authService)
        {
            var path = httpContext.Request.Path.Value;
            var request = httpContext.Request;
            var response = httpContext.Response;

            if (ShouldValidateToken(path) && HasValidTokens(request.Cookies, out var accessToken, out var refreshToken))
            {
                var refAccessToken = request.Cookies["REF-TOKEN"]?.Trim();
                var xHeader = request.Headers["x-header"];

                if (IsValidToken(_authService, accessToken, refreshToken, refAccessToken, xHeader))
                {
                    request.Headers.Append("Authorization", "Bearer " + accessToken);
                }
                else
                {
                    RevokeTokens(_authService, refreshToken, accessToken);
                    ClearCookies(request.Cookies, response.Cookies);

                    logger.LogDebug($"AuthMiddleware > Invalid user access token {accessToken} and refresh token {refreshToken}");
                }
            }
            else if (path.Contains("/authenticate/LogoutAsync", StringComparison.OrdinalIgnoreCase) && request.Cookies.ContainsKey("ACCESS-TOKEN"))
            {
                var token = request.Cookies["ACCESS-TOKEN"].Trim();
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Append("Authorization", "Bearer " + token);
                }
            }

            await _next(httpContext);
        }

        private static bool ShouldValidateToken(string path)
        {
            var excludedPaths = new[]
            {
            "/authenticate/login",
            "/authenticate/LogoutAsync",
            "/notifications/serverdatetime",
            "/notifications/buildnumber",
            "/api/public/v1/media"
         };

            return !excludedPaths.ToList().Exists(excludedPath => path.Contains(excludedPath, StringComparison.OrdinalIgnoreCase));
        }

        private static bool HasValidTokens(IRequestCookieCollection cookies, out string accessToken, out string refreshToken)
        {
            accessToken = cookies["ACCESS-TOKEN"]?.Trim();
            refreshToken = cookies["REFRESH-TOKEN"]?.Trim();
            return !string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken);
        }

        private static bool IsValidToken(IAuthService authService, string accessToken, string refreshToken, string refAccessToken, string xHeader)
        {
            var userToken = authService.IsTokenValid(accessToken, refreshToken, refAccessToken);
            return userToken != null && userToken.SessionId == xHeader;
        }

        private static void RevokeTokens(IAuthService authService, string refreshToken, string accessToken)
        {
            authService.RevokeToken(refreshToken, accessToken, "", false);
        }

        private static void ClearCookies(IRequestCookieCollection requestCookies, IResponseCookies responseCookies)
        {
            foreach (var cookie in requestCookies.Keys)
            {
                responseCookies.Delete(cookie);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class AuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
