using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;

namespace LicensingAndTransfer.API
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AuthorizationAttribute : System.Web.Http.Filters.AuthorizationFilterAttribute
    {
        /// <summary>
        /// Method invoked before executing the actual web method
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            Constants.Log.InfoFormat("Controller-{0} Method-{1} | Initiate", actionContext.ActionDescriptor.ControllerDescriptor.ControllerName, actionContext.ActionDescriptor.ActionName);
            if (actionContext.Request != null && actionContext.Request.Headers != null && actionContext.Request.Headers.Authorization != null
                && actionContext.Request.Headers.Authorization.Parameter != null)
            {
                Constants.Log.DebugFormat("Controller-{0} Method-{1} | Parameter: {2}", actionContext.ActionDescriptor.ControllerDescriptor.ControllerName, actionContext.ActionDescriptor.ActionName, actionContext.Request.Headers.Authorization.Parameter);
                Constants.Log.DebugFormat("Controller-{0} Method-{1} | Scheme: {2}", actionContext.ActionDescriptor.ControllerDescriptor.ControllerName, actionContext.ActionDescriptor.ActionName, actionContext.Request.Headers.Authorization.Scheme);
            }
            base.OnAuthorization(actionContext);
        }
    }
}