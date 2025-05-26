using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace LicensingAndTransfer.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API configuration and services
            config.Filters.Add(new ActionExecutionAttribute()); // Global level // Enable execution trace on each method in the WebAPI

            // Web API configuration and services
            config.Filters.Add(new AuthorizationAttribute()); // Global level // Enable execution trace on each Authorized method in the WebAPI
            
            // Web API configuration and services
            config.Filters.Add(new CustomExceptionAttribute()); // Global level // Enable custom exception for the WebAPI


            // Web API configuration and services
           // config.Filters.Add(new TokenAuthenticationAttribute());     // Global level //  Enable authentication for the WebAPI
          
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //  Converts a System.DateTime to and from the ISO 8601 date format (e.g. 2008-04-12T12:53Z).
            //  This will help to align the date format with ISO 8601 i.e., "yyyy-MM-ddThh:mm:ss.fffZ"
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.IsoDateTimeConverter());

            // To make sure the ENUMs are converted to string while exchanging in API calls
            //  config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());

            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Remove(config.Formatters.FormUrlEncodedFormatter);
            config.Formatters.Clear();
            config.Formatters.Add(new System.Net.Http.Formatting.JsonMediaTypeFormatter());
        }
    }
}
