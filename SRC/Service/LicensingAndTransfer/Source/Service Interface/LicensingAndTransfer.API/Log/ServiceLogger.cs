using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LicensingAndTransfer.API.DataAccess;

namespace LicensingAndTransfer.API
{
    /// <summary>
    /// Track every request and response of the WebAPI and save in the DB
    /// </summary>
    public class ServiceLogger : IDisposable
    {
        /// <summary>
        /// Save the information to database
        /// </summary>
        /// <param name="Request">Request Object</param>
        /// <param name="Module">Module Name</param>
        /// <param name="MethodType">Method Type - Insert / Update / Delete / Select</param>
        /// <param name="StartTime">Start Time of the method call</param>
        /// <param name="EndTime">Method execution completed time</param>
        /// <param name="ErrorMsg">Error details captured during method execution</param>
        /// <param name="status">Final status of method execution</param>
        /// <param name="Response">Response Object</param>
        /// <param name="logID">Log Identity</param>
        /// <returns></returns>
        public Int64 SaveLog(object Request, string Module, string MethodType, DateTime StartTime, DateTime EndTime, string ErrorMsg, string status, object Response, Int64 logID)
        {
            string Jsonrequest = string.Empty, Jsonresponse = string.Empty;
            Int64 WebServicelogID = -1;
            try
            {
                if (Request != null)
                    Jsonrequest = Newtonsoft.Json.JsonConvert.SerializeObject(Request, Newtonsoft.Json.Formatting.None);

                if (Response != null)
                    Jsonresponse = Newtonsoft.Json.JsonConvert.SerializeObject(Response, Newtonsoft.Json.Formatting.None);

                using (IntegrationDAL objDAL = new IntegrationDAL())
                {
                    WebServicelogID = objDAL.SaveLog(Jsonrequest, Module, MethodType, StartTime, EndTime, ErrorMsg, status, Jsonresponse, logID);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not save log", ex);
            }
            finally { Jsonrequest = Jsonresponse = string.Empty; }
            return WebServicelogID;
        }

        /// <summary>
        /// Disposing the class reference
        /// </summary>
        public void Dispose()
        { }
    }
}