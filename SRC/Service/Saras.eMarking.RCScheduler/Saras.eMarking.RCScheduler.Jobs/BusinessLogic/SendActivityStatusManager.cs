using Saras.eMarking.RCScheduler.Jobs.DataAccess;
using Saras.eMarking.RCScheduler.Jobs.Utilities;
using Saras.SystemFramework.Core.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Saras.eMarking.RCScheduler.Jobs.Utilities.GenericApiCallHandler;


namespace Saras.eMarking.RCScheduler.Jobs.BusinessLogic
{
    public static class SendActivityStatusManager
    {
        static readonly SarasLogger Log = new SarasLogger(typeof(SendEmailManager));

        public static string CallActivityStatusEmail(string triggerName,string SchedulerName)
        {

            string ApiResponse = string.Empty;
            string ApiUrl = string.Empty;
            int processStatus = 0;
            try
            {
                ApiUrl = string.Concat(ConfigurationManager.AppSettings.Get("UserSyncApiUrl"), "api/eMarking/notify");
                ApiResponse = CallRestApi<string, string>(ApiUrl, string.Empty, HttpMethodType.Post);
                processStatus = 1;
                return ApiResponse;
            }
            catch (Exception ex)
            {
                processStatus = 2;
                Log.LogError(string.Format("Error in SendActivityStatusManager CallActivityStatusEmail Url : {0} and Exception Message : {1}", ApiUrl, ex.Message), ex);
            }
            finally
            {
                RcCheckDA rcCheckDA = new RcCheckDA();
                rcCheckDA.RcJobTrackingHistory(SchedulerName, triggerName, triggerName, triggerName, 0, 0, 0, 0, 0, DateTime.UtcNow, null, ApiResponse, processStatus);
            }

            return ApiResponse;
        }

    }
}
