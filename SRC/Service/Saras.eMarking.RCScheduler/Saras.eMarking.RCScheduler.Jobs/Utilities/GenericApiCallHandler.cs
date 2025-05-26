using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Saras.SystemFramework.Core.Logging;

namespace Saras.eMarking.RCScheduler.Jobs.Utilities
{
    public static class GenericApiCallHandler
    {
        static SarasLogger Log = new SarasLogger(typeof(GenericApiCallHandler));

        public static string CallRestApi<T1, T2>(string ApiUrl, T2 ApiRequestObject, string ApiMethodType, string ContentType = "application/json", int timeoutSeconds = 500)
        {
            //T1 ResponseData = default(T1);
            string strJSONText = string.Empty;
            try
            {
                var settings = new JsonSerializerSettings();
                settings.TypeNameHandling = TypeNameHandling.All;
                string ApiRequestPostData = JsonConvert.SerializeObject(ApiRequestObject, Formatting.Indented, settings);
                switch (ApiMethodType)
                {
                    case HttpMethodType.Post:
                        strJSONText = HttpClientRequestHandler.PostApiHandler(ApiUrl, ApiRequestPostData, ContentType, timeoutSeconds);
                        break;
                    case HttpMethodType.Put:
                        strJSONText = HttpClientRequestHandler.PutApiHandler(ApiUrl, ApiRequestPostData, ContentType, timeoutSeconds);
                        break;
                    default:
                        strJSONText = HttpClientRequestHandler.GetApiHandler(ApiUrl, ApiRequestPostData, ContentType, timeoutSeconds);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.LogError(string.Format("Error in CallApiHTTPRequest CallApiHTTPRequest Url : {0} and Exception Message : {1}", ApiUrl, ex.Message), ex);
            }
            return strJSONText;
        }

    }
    public static class HttpMethodType
    {
        //
        // Summary:
        //     Represents an HTTP GET protocol method.
        public const string Get = "GET";
        //
        // Summary:
        //     Represents an HTTP PUT protocol method that is used to replace an entity identified
        //     by a URI.
        public const string Put = "PUT";
        // Summary:
        //     Represents an HTTP POST protocol method that is used to post a new entity as
        //     an addition to a URI.
        public const string Post = "POST";
    }
}
