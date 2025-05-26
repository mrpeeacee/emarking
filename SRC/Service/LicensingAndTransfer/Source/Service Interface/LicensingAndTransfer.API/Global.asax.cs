using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace LicensingAndTransfer.API
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private bool development;
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            if (app != null && app.Context != null && app.Context.Response != null && app.Context.Response.Headers != null)
            {
                //  "Banner Grabbing" technique shares the type and version of software; We need to block it and hence using the below code
                if (!string.IsNullOrEmpty(app.Context.Response.Headers.Get("Server")))
                    app.Context.Response.Headers.Remove("Server");
                if (!string.IsNullOrEmpty(app.Context.Response.Headers.Get("X-Powered-By")))
                    app.Context.Response.Headers.Remove("X-Powered-By");
            }
        }

        protected void Application_Start()
        {
            //  "Banner Grabbing" technique shares the type and version of software; We need to block it and hence using the below code
            MvcHandler.DisableMvcResponseHeader = true;

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, Errors) => 
            {
                if (development) return true;
                return Errors == SslPolicyErrors.None;
            };




            System.Net.ServicePointManager.Expect100Continue = false;

            Newtonsoft.Json.JsonConvert.DefaultSettings = () => new Newtonsoft.Json.JsonSerializerSettings
            {
                //  This will help the response to ignore attribute with null values
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                        
                //  This will help to align the date format with ISO 8601 i.e., "yyyy-MM-ddThh:mm:ss.fffZ"
                DateFormatHandling = Newtonsoft.Json.DateFormatHandling.IsoDateFormat,

                //  Enfore every attribute to align with ISO 8601 format
                Converters = { new Newtonsoft.Json.Converters.IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-ddThh:mm:ss.fffZ" } },

                //  Log exceptions when JSON cannot be serialized/deserialized
                Error = delegate(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e) { Constants.Log.Error("Error in Json Serialization", e.ErrorContext.Error); }
            };

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            try
            {
                System.IO.FileInfo finfo = new System.IO.FileInfo(System.Web.HttpContext.Current.Server.MapPath(".") + "\\Log\\Log4Net.config");
                if (finfo.Exists)
                    log4net.Config.XmlConfigurator.ConfigureAndWatch(finfo);

            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog objEventLog = new System.Diagnostics.EventLog();
                ((System.ComponentModel.ISupportInitialize)objEventLog).BeginInit();
                objEventLog.Log = "Application - " + ex.Message;
                objEventLog.Source = "Saras";
                ((System.ComponentModel.ISupportInitialize)objEventLog).EndInit();
            }
        }
    }
}
