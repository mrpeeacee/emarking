using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace LicensingAndTransfer.API
{
    /// <summary>
    /// Implements exception filter methods to log exception details
    /// </summary>
    public class CustomExceptionAttribute : System.Web.Http.Filters.ExceptionFilterAttribute
    {
        /// <summary>
        /// Override the method to log exception
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            Constants.Log.Error(string.Format("Controller-{0} Method-{1} | Error", actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName , actionExecutedContext.ActionContext.ActionDescriptor.ActionName), actionExecutedContext.Exception);

            //Constants.SMTP.SendMail(actionExecutedContext.ActionContext.ActionDescriptor.ActionName, Constants.MailAddressTo, Constants.MailAddressCc,
            //    string.Format("Error in {0}", actionExecutedContext.ActionContext.ActionDescriptor.ActionName),
            //    string.Format("Error in {0}", actionExecutedContext.ActionContext.ActionDescriptor.ActionName),
            //    actionExecutedContext.Exception);

            base.OnException(actionExecutedContext);
        }
    }
}