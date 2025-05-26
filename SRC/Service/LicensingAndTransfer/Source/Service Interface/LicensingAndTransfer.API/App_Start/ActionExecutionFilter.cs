using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Linq;
using System.Collections.Generic;

namespace LicensingAndTransfer.API
{
    /// <summary>
    /// Implement the action filter, and track every incoming request. If needed, use it to log inbound and outbound data.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ActionExecutionAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        private const string StartDateTime = "StartDateTime", EndDateTime = "EndDateTime", Logger = "Logger", LogID = "LogID";
        /// <summary>
        /// Method invoked before executing the actual web method
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {

            if (actionContext.ModelState.IsValid == false)
            {
                actionContext.Response = actionContext.Request.CreateResponse<List<Status>>(System.Net.HttpStatusCode.BadRequest,
                   //(actionContext.ModelState
                   // .Where(modelError => modelError.Value.Errors.Count > 0)
                   // .Select(modelError => new Status
                   // {
                   //     Code = modelError.Value.Errors.LastOrDefault().ErrorMessage.Any() ? modelError.Value.Errors.LastOrDefault().ErrorMessage.Substring(0, modelError.Value.Errors.LastOrDefault().ErrorMessage.IndexOf(":")) : null,
                   //     Reason = modelError.Value.Errors.LastOrDefault().ErrorMessage.Any() ? modelError.Value.Errors.LastOrDefault().ErrorMessage.Substring(modelError.Value.Errors.LastOrDefault().ErrorMessage.IndexOf(":") + 1) : null
                   // })
                   // .ToList())
                   actionContext.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Where(y => y.ErrorMessage.Contains(":"))
                    .Select(z =>
                        new Status
                        {
                            Code = z.ErrorMessage.Substring(0, z.ErrorMessage.IndexOf(":")),
                            Reason = z.ErrorMessage.Substring(z.ErrorMessage.IndexOf(":") + 1)
                        })
                    .Union(
                           (
                            // Handle system exception when invalid data is sent
                            from ms in actionContext.ModelState.Values
                            where ms.Errors != null && ms.Errors.FirstOrDefault() != null && ms.Errors.FirstOrDefault().Exception != null
                            select new Status() { Code = "UC002", Reason = "Supplied input is invalid" }
                            ).Take(1)
                       ).ToList()
                    );
            }
            else
            {
                Constants.Log.InfoFormat("Controller-{0} Method-{1} | Initiate", actionContext.ActionDescriptor.ControllerDescriptor.ControllerName, actionContext.ActionDescriptor.ActionName);
                foreach (var entryItem in actionContext.ActionArguments)
                {
                    Constants.Log.DebugFormat("Controller-{0} Method-{1} | InputParam-{2}", actionContext.ActionDescriptor.ControllerDescriptor.ControllerName, actionContext.ActionDescriptor.ActionName, entryItem.Key);
                    Constants.Log.Debug(entryItem.Value);
                }
                actionContext.ActionDescriptor.Properties[StartDateTime] = DateTime.Now;
                actionContext.ActionDescriptor.Properties[EndDateTime] = DateTime.Now;
                actionContext.ActionDescriptor.Properties[Logger] = new ServiceLogger();

                ServiceLogger serviceLogger = actionContext.ActionDescriptor.Properties[Logger] as ServiceLogger;
                actionContext.ActionDescriptor.Properties[LogID] = serviceLogger.SaveLog(actionContext.ActionArguments, actionContext.ActionDescriptor.ControllerDescriptor.ControllerName, "",
                    Convert.ToDateTime(actionContext.ActionDescriptor.Properties[StartDateTime]), Convert.ToDateTime(actionContext.ActionDescriptor.Properties[EndDateTime]),
                    null, null, null, 0);
                serviceLogger.Dispose();

                base.OnActionExecuting(actionContext);
            }
        }

        /// <summary>
        /// Method invoked after completing the execution of the actual web method
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception == null)
            {
                if (actionExecutedContext.Response.Content == null)
                    Constants.Log.Debug(actionExecutedContext.Response.StatusCode);
                else
                    Constants.Log.Debug(actionExecutedContext.Response.Content.ReadAsStringAsync().Result);
                Constants.Log.InfoFormat("Controller-{0} Method-{1} | Executed", actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName, actionExecutedContext.ActionContext.ActionDescriptor.ActionName);

                actionExecutedContext.ActionContext.ActionDescriptor.Properties[EndDateTime] = DateTime.Now;

                ServiceLogger serviceLogger = actionExecutedContext.ActionContext.ActionDescriptor.Properties[Logger] as ServiceLogger;
                serviceLogger.SaveLog(actionExecutedContext.ActionContext.ActionArguments, actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                    actionExecutedContext.Request.Method.ToString(),
                    Convert.ToDateTime(actionExecutedContext.ActionContext.ActionDescriptor.Properties[StartDateTime]),
                    Convert.ToDateTime(actionExecutedContext.ActionContext.ActionDescriptor.Properties[EndDateTime]),
                    actionExecutedContext.Exception == null ? null : actionExecutedContext.Exception.Message,
                    actionExecutedContext.Exception == null ? "S001" : "F001",
                    ((actionExecutedContext.Response.Content == null) ? "" : actionExecutedContext.Response.Content.ReadAsStringAsync().Result),
                    Convert.ToInt64(actionExecutedContext.ActionContext.ActionDescriptor.Properties[LogID]));
                serviceLogger.Dispose();
            }

            base.OnActionExecuted(actionExecutedContext);
        }
    }
}