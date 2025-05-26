using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;

namespace LicensingAndTransfer.API
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            //  Constants.Log.Info("Initialize ApplicationOAuthProvider");
            //  Constants.Log.Debug(publicClientId);
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            //  Constants.Log.Info("Executing GrantResourceOwnerCredentials");
            //  Constants.Log.Debug($"ClientId: {context.ClientId} | Error: {context.Error} | ErrorDescription: {context.ErrorDescription} | ErrorUri: {context.ErrorUri} | HasError: {context.HasError} | IsValidated: {context.IsValidated} | AuthenticationType: {context.Options.AuthenticationType} | TokenEndpointPath: {context.Options.TokenEndpointPath} | Password: {context.Password} | Method: {context.Request.Method} | Path: {context.Request.Path} | PathBase: {context.Request.PathBase} | Protocol: {context.Request.Protocol} | ResponseType-ContentType: {context.Response.ContentType} | ResponseType-ReasonPhrase: {context.Response.ReasonPhrase} | ResponseType-StatusCode: {context.Response.StatusCode}");

            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            //  Constants.Log.Info("Executing GrantResourceOwnerCredentials userManager");
            //  Constants.Log.Debug($"userManager: {userManager} | ClientId: {context.ClientId} | Error: {context.Error} | ErrorDescription: {context.ErrorDescription} | ErrorUri: {context.ErrorUri} | HasError: {context.HasError} | IsValidated: {context.IsValidated} | AuthenticationType: {context.Options.AuthenticationType} | TokenEndpointPath: {context.Options.TokenEndpointPath} | Password: {context.Password} | Method: {context.Request.Method} | Path: {context.Request.Path} | PathBase: {context.Request.PathBase} | Protocol: {context.Request.Protocol} | ResponseType-ContentType: {context.Response.ContentType} | ResponseType-ReasonPhrase: {context.Response.ReasonPhrase} | ResponseType-StatusCode: {context.Response.StatusCode}");

            ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);
            //  Constants.Log.Info("Executing GrantResourceOwnerCredentials user");
            //  Constants.Log.Debug($"user: {user} | ClientId: {context.ClientId} | Error: {context.Error} | ErrorDescription: {context.ErrorDescription} | ErrorUri: {context.ErrorUri} | HasError: {context.HasError} | IsValidated: {context.IsValidated} | AuthenticationType: {context.Options.AuthenticationType} | TokenEndpointPath: {context.Options.TokenEndpointPath} | Password: {context.Password} | Method: {context.Request.Method} | Path: {context.Request.Path} | PathBase: {context.Request.PathBase} | Protocol: {context.Request.Protocol} | ResponseType-ContentType: {context.Response.ContentType} | ResponseType-ReasonPhrase: {context.Response.ReasonPhrase} | ResponseType-StatusCode: {context.Response.StatusCode}");

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
               OAuthDefaults.AuthenticationType);
            //  Constants.Log.Info("Executing GrantResourceOwnerCredentials oAuthIdentity");
            //  Constants.Log.Debug($"oAuthIdentity: {oAuthIdentity} | ClientId: {context.ClientId} | Error: {context.Error} | ErrorDescription: {context.ErrorDescription} | ErrorUri: {context.ErrorUri} | HasError: {context.HasError} | IsValidated: {context.IsValidated} | AuthenticationType: {context.Options.AuthenticationType} | TokenEndpointPath: {context.Options.TokenEndpointPath} | Password: {context.Password} | Method: {context.Request.Method} | Path: {context.Request.Path} | PathBase: {context.Request.PathBase} | Protocol: {context.Request.Protocol} | ResponseType-ContentType: {context.Response.ContentType} | ResponseType-ReasonPhrase: {context.Response.ReasonPhrase} | ResponseType-StatusCode: {context.Response.StatusCode}");

            ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                CookieAuthenticationDefaults.AuthenticationType);
            //  Constants.Log.Info("Executing GrantResourceOwnerCredentials cookiesIdentity");
            //  Constants.Log.Debug($"cookiesIdentity: {cookiesIdentity} | ClientId: {context.ClientId} | Error: {context.Error} | ErrorDescription: {context.ErrorDescription} | ErrorUri: {context.ErrorUri} | HasError: {context.HasError} | IsValidated: {context.IsValidated} | AuthenticationType: {context.Options.AuthenticationType} | TokenEndpointPath: {context.Options.TokenEndpointPath} | Password: {context.Password} | Method: {context.Request.Method} | Path: {context.Request.Path} | PathBase: {context.Request.PathBase} | Protocol: {context.Request.Protocol} | ResponseType-ContentType: {context.Response.ContentType} | ResponseType-ReasonPhrase: {context.Response.ReasonPhrase} | ResponseType-StatusCode: {context.Response.StatusCode}");

            AuthenticationProperties properties = CreateProperties(user.UserName);

            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            //  Constants.Log.Info("Executing GrantResourceOwnerCredentials ticket");
            //  Constants.Log.Debug($"ticket: {ticket} | ClientId: {context.ClientId} | Error: {context.Error} | ErrorDescription: {context.ErrorDescription} | ErrorUri: {context.ErrorUri} | HasError: {context.HasError} | IsValidated: {context.IsValidated} | AuthenticationType: {context.Options.AuthenticationType} | TokenEndpointPath: {context.Options.TokenEndpointPath} | Password: {context.Password} | Method: {context.Request.Method} | Path: {context.Request.Path} | PathBase: {context.Request.PathBase} | Protocol: {context.Request.Protocol} | ResponseType-ContentType: {context.Response.ContentType} | ResponseType-ReasonPhrase: {context.Response.ReasonPhrase} | ResponseType-StatusCode: {context.Response.StatusCode}");

            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            //  Constants.Log.Info("Executing TokenEndpoint");
            //  Constants.Log.Debug($"AuthenticationType: {context.Options.AuthenticationType} | TokenEndpointPath: {context.Options.TokenEndpointPath} | Method: {context.Request.Method} | Path: {context.Request.Path} | PathBase: {context.Request.PathBase} | Protocol: {context.Request.Protocol} | ResponseType-ContentType: {context.Response.ContentType} | ResponseType-ReasonPhrase: {context.Response.ReasonPhrase} | ResponseType-StatusCode: {context.Response.StatusCode}");
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            //  Constants.Log.Info("Executing TokenEndpoint context");

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            //  Constants.Log.Info("Executing ValidateClientAuthentication");
            //  Constants.Log.Debug($"ClientId: {context.ClientId} | Error: {context.Error} | ErrorDescription: {context.ErrorDescription} | ErrorUri: {context.ErrorUri} | HasError: {context.HasError} | IsValidated: {context.IsValidated} | AuthenticationType: {context.Options.AuthenticationType} | TokenEndpointPath: {context.Options.TokenEndpointPath} | Method: {context.Request.Method} | Path: {context.Request.Path} | PathBase: {context.Request.PathBase} | Protocol: {context.Request.Protocol} | ResponseType-ContentType: {context.Response.ContentType} | ResponseType-ReasonPhrase: {context.Response.ReasonPhrase} | ResponseType-StatusCode: {context.Response.StatusCode}");

            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }
            //  Constants.Log.Info("Executing ValidateClientAuthentication context");
            //  Constants.Log.Debug($"ClientId: {context.ClientId} | Error: {context.Error} | ErrorDescription: {context.ErrorDescription} | ErrorUri: {context.ErrorUri} | HasError: {context.HasError} | IsValidated: {context.IsValidated} | AuthenticationType: {context.Options.AuthenticationType} | TokenEndpointPath: {context.Options.TokenEndpointPath} | Method: {context.Request.Method} | Path: {context.Request.Path} | PathBase: {context.Request.PathBase} | Protocol: {context.Request.Protocol} | ResponseType-ContentType: {context.Response.ContentType} | ResponseType-ReasonPhrase: {context.Response.ReasonPhrase} | ResponseType-StatusCode: {context.Response.StatusCode}");

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            //  Constants.Log.Info("Executing ValidateClientRedirectUri");
            //  Constants.Log.Debug($"ClientId: {context.ClientId} | Error: {context.Error} | ErrorDescription: {context.ErrorDescription} | ErrorUri: {context.ErrorUri} | HasError: {context.HasError} | IsValidated: {context.IsValidated} | AuthenticationType: {context.Options.AuthenticationType} | TokenEndpointPath: {context.Options.TokenEndpointPath} | Method: {context.Request.Method} | Path: {context.Request.Path} | PathBase: {context.Request.PathBase} | Protocol: {context.Request.Protocol} | ResponseType-ContentType: {context.Response.ContentType} | ResponseType-ReasonPhrase: {context.Response.ReasonPhrase} | ResponseType-StatusCode: {context.Response.StatusCode}");

            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }
            //  Constants.Log.Info("Executing ValidateClientRedirectUri context");
            //  Constants.Log.Debug($"ClientId: {context.ClientId} | Error: {context.Error} | ErrorDescription: {context.ErrorDescription} | ErrorUri: {context.ErrorUri} | HasError: {context.HasError} | IsValidated: {context.IsValidated} | AuthenticationType: {context.Options.AuthenticationType} | TokenEndpointPath: {context.Options.TokenEndpointPath} | Method: {context.Request.Method} | Path: {context.Request.Path} | PathBase: {context.Request.PathBase} | Protocol: {context.Request.Protocol} | ResponseType-ContentType: {context.Response.ContentType} | ResponseType-ReasonPhrase: {context.Response.ReasonPhrase} | ResponseType-StatusCode: {context.Response.StatusCode}");

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(string userName)
        {
            //  Constants.Log.Info("Executing CreateProperties");
            //  Constants.Log.Debug($"userName: {userName}");

            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userName", userName }
            };
            return new AuthenticationProperties(data);
        }
        /*
        public override Task AuthorizationEndpointResponse(OAuthAuthorizationEndpointResponseContext context)
        {
            Constants.Log.Info("Executing AuthorizationEndpointResponse");
            Constants.Log.Debug($"AccessToken: {context.AccessToken} | AuthorizationCode: {context.AuthorizationCode} | IsRequestCompleted: {context.IsRequestCompleted} | AuthenticationType: {context.Options.AuthenticationType} | TokenEndpointPath: {context.Options.TokenEndpointPath} | Method: {context.Request.Method} | Path: {context.Request.Path} | PathBase: {context.Request.PathBase} | Protocol: {context.Request.Protocol} | ResponseType-ContentType: {context.Response.ContentType} | ResponseType-ReasonPhrase: {context.Response.ReasonPhrase} | ResponseType-StatusCode: {context.Response.StatusCode}");

            return base.AuthorizationEndpointResponse(context);
        }
        public override Task AuthorizeEndpoint(OAuthAuthorizeEndpointContext context)
        {
            Constants.Log.Info("Executing AuthorizeEndpoint");
            Constants.Log.Debug($"AuthorizeRequest: {context.AuthorizeRequest}");

            return base.AuthorizeEndpoint(context);
        }
        public override Task GrantAuthorizationCode(OAuthGrantAuthorizationCodeContext context)
        {
            Constants.Log.Info("Executing GrantAuthorizationCode");
            Constants.Log.Debug($"Error: {context.Error} | ErrorDescription: {context.ErrorDescription} | ErrorUri: {context.ErrorUri} | AuthenticationType: {context.Options.AuthenticationType} | TokenEndpointPath: {context.Options.TokenEndpointPath} | Method: {context.Request.Method} | Path: {context.Request.Path} | PathBase: {context.Request.PathBase} | Protocol: {context.Request.Protocol} | ResponseType-ContentType: {context.Response.ContentType} | ResponseType-ReasonPhrase: {context.Response.ReasonPhrase} | ResponseType-StatusCode: {context.Response.StatusCode}");

            return base.GrantAuthorizationCode(context);
        }
        public override Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            Constants.Log.Info("Executing GrantClientCredentials");
            Constants.Log.Debug($"Error: {context.Error} | ErrorDescription: {context.ErrorDescription} | ErrorUri: {context.ErrorUri} | AuthenticationType: {context.Options.AuthenticationType} | TokenEndpointPath: {context.Options.TokenEndpointPath} | Method: {context.Request.Method} | Path: {context.Request.Path} | PathBase: {context.Request.PathBase} | Protocol: {context.Request.Protocol} | ResponseType-ContentType: {context.Response.ContentType} | ResponseType-ReasonPhrase: {context.Response.ReasonPhrase} | ResponseType-StatusCode: {context.Response.StatusCode}");

            return base.GrantClientCredentials(context);
        }
        public override Task GrantCustomExtension(OAuthGrantCustomExtensionContext context)
        {
            Constants.Log.Info("Executing GrantCustomExtension");
            Constants.Log.Debug($"Error: {context.Error} | ErrorDescription: {context.ErrorDescription} | ErrorUri: {context.ErrorUri} | AuthenticationType: {context.Options.AuthenticationType} | TokenEndpointPath: {context.Options.TokenEndpointPath} | Method: {context.Request.Method} | Path: {context.Request.Path} | PathBase: {context.Request.PathBase} | Protocol: {context.Request.Protocol} | ResponseType-ContentType: {context.Response.ContentType} | ResponseType-ReasonPhrase: {context.Response.ReasonPhrase} | ResponseType-StatusCode: {context.Response.StatusCode}");

            return base.GrantCustomExtension(context);
        }
        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            Constants.Log.Info("Executing GrantRefreshToken");
            Constants.Log.Debug($"AuthenticationType: {context.Options.AuthenticationType} | TokenEndpointPath: {context.Options.TokenEndpointPath} | Method: {context.Request.Method} | Path: {context.Request.Path} | PathBase: {context.Request.PathBase} | Protocol: {context.Request.Protocol} | ResponseType-ContentType: {context.Response.ContentType} | ResponseType-ReasonPhrase: {context.Response.ReasonPhrase} | ResponseType-StatusCode: {context.Response.StatusCode}");

            return base.GrantRefreshToken(context);
        }
        public override Task MatchEndpoint(OAuthMatchEndpointContext context)
        {
            Constants.Log.Info("Executing MatchEndpoint");
            Constants.Log.Debug($"IsRequestCompleted: {context.IsRequestCompleted} | AuthenticationType: {context.Options.AuthenticationType} | TokenEndpointPath: {context.Options.TokenEndpointPath} | Method: {context.Request.Method} | Path: {context.Request.Path} | PathBase: {context.Request.PathBase} | Protocol: {context.Request.Protocol} | ResponseType-ContentType: {context.Response.ContentType} | ResponseType-ReasonPhrase: {context.Response.ReasonPhrase} | ResponseType-StatusCode: {context.Response.StatusCode}");

            return base.MatchEndpoint(context);
        }
        public override Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        {
            Constants.Log.Info("Executing TokenEndpointResponse");
            Constants.Log.Debug($"AccessToken: {context.AccessToken} | AdditionalResponseParameters: {context.AdditionalResponseParameters} | AuthenticationType: {context.Options.AuthenticationType} | TokenEndpointPath: {context.Options.TokenEndpointPath} | Method: {context.Request.Method} | Path: {context.Request.Path} | PathBase: {context.Request.PathBase} | Protocol: {context.Request.Protocol} | ResponseType-ContentType: {context.Response.ContentType} | ResponseType-ReasonPhrase: {context.Response.ReasonPhrase} | ResponseType-StatusCode: {context.Response.StatusCode}");

            return base.TokenEndpointResponse(context);
        }
        public override Task ValidateAuthorizeRequest(OAuthValidateAuthorizeRequestContext context)
        {
            Constants.Log.Info("Executing ValidateAuthorizeRequest");
            Constants.Log.Debug($"Error: {context.Error} | ErrorDescription: {context.ErrorDescription} | ErrorUri: {context.ErrorUri} | AuthenticationType: {context.Options.AuthenticationType} | TokenEndpointPath: {context.Options.TokenEndpointPath} | Method: {context.Request.Method} | Path: {context.Request.Path} | PathBase: {context.Request.PathBase} | Protocol: {context.Request.Protocol} | ResponseType-ContentType: {context.Response.ContentType} | ResponseType-ReasonPhrase: {context.Response.ReasonPhrase} | ResponseType-StatusCode: {context.Response.StatusCode}");

            return base.ValidateAuthorizeRequest(context);
        }
        public override Task ValidateTokenRequest(OAuthValidateTokenRequestContext context)
        {
            Constants.Log.Info("Executing ValidateTokenRequest");
            Constants.Log.Debug($"Error: {context.Error} | ErrorDescription: {context.ErrorDescription} | ErrorUri: {context.ErrorUri} | AuthenticationType: {context.Options.AuthenticationType} | TokenEndpointPath: {context.Options.TokenEndpointPath} | Method: {context.Request.Method} | Path: {context.Request.Path} | PathBase: {context.Request.PathBase} | Protocol: {context.Request.Protocol} | ResponseType-ContentType: {context.Response.ContentType} | ResponseType-ReasonPhrase: {context.Response.ReasonPhrase} | ResponseType-StatusCode: {context.Response.StatusCode}");

            return base.ValidateTokenRequest(context);
        }
        */
    }
}