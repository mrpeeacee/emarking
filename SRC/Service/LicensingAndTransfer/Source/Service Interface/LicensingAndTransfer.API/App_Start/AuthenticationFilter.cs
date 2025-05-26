using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LicensingAndTransfer.API
{
    /// <summary>
    /// Custom Authentication Class built to authenticate every incoming WebAPI requests
    /// MSDN Reference: https://msdn.microsoft.com/magazine/dn781361.aspx
    /// </summary>
    public class TokenAuthenticationAttribute : Attribute, System.Web.Http.Filters.IAuthenticationFilter
    {
        /// <summary>
        /// When enabled multiple instance are allowed
        /// </summary>
        public bool AllowMultiple { get { return false; } }
        // The AuthenticateAsync and ChallengeAsync methods go here

        /// <summary>
        /// Authenticates the request
        /// </summary>
        /// <param name="context">The authentication context</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        /// <returns></returns>
        public System.Threading.Tasks.Task AuthenticateAsync(System.Web.Http.Filters.HttpAuthenticationContext context, System.Threading.CancellationToken cancellationToken)
        {
            var req = context.Request;
            // Get credential from the Authorization header (if present) and authenticate
            if (req.Headers.Authorization != null && Constants.Scheme.Equals(req.Headers.Authorization.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                if (LicensingAndTransfer.API.Libraries.Token.Validate(req.Headers.Authorization.Parameter))  //  Validate received Token
                {
                    var claims = new List<System.Security.Claims.Claim>()
                      {
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "excelsoft"),
                        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, "admin")
                      };
                    var id = new System.Security.Claims.ClaimsIdentity(claims, "Token");
                    var principal = new System.Security.Claims.ClaimsPrincipal(new[] { id });
                    // The request message contains valid credential
                    context.Principal = principal;
                }
                else
                {
                    // The request message contains invalid credential
                    context.ErrorResult = new System.Web.Http.Results.UnauthorizedResult(
                      new System.Net.Http.Headers.AuthenticationHeaderValue[0], context.Request);
                }
            }
            return System.Threading.Tasks.Task.FromResult(0);
        }

        /// <summary>
        /// Method helping the authentication context
        /// </summary>
        /// <param name="context">The authentication context</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
        /// <returns></returns>
        public System.Threading.Tasks.Task ChallengeAsync(System.Web.Http.Filters.HttpAuthenticationChallengeContext context, System.Threading.CancellationToken cancellationToken)
        {
            context.Result = new ResultWithChallenge(context.Result);
            return System.Threading.Tasks.Task.FromResult(0);
        }

        /// <summary>
        /// WebAPI Result authenticating class
        /// </summary>
        public class ResultWithChallenge : System.Web.Http.IHttpActionResult
        {
            private readonly System.Web.Http.IHttpActionResult next;

            /// <summary>
            /// Authenticating the result
            /// </summary>
            /// <param name="next"></param>
            public ResultWithChallenge(System.Web.Http.IHttpActionResult next)
            {
                this.next = next;
            }

            /// <summary>
            /// Creates an System.Net.Http.HttpResponseMessage asynchronously
            /// </summary>
            /// <param name="cancellationToken">The token to monitor for cancellation requests</param>
            /// <returns></returns>
            public async System.Threading.Tasks.Task<System.Net.Http.HttpResponseMessage> ExecuteAsync(System.Threading.CancellationToken cancellationToken)
            {
                var response = await next.ExecuteAsync(cancellationToken);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    response.Headers.WwwAuthenticate.Add(new System.Net.Http.Headers.AuthenticationHeaderValue(Constants.Scheme, "realm=\"Access to the site\", charset=\"UTF - 8\""));
                }
                return response;
            }
        }
    }
}