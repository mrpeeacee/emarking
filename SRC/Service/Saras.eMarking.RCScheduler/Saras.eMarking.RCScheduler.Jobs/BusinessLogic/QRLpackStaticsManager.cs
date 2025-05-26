using System;
using System.Configuration;
using Saras.eMarking.RCScheduler.Jobs.DataAccess;
using Saras.eMarking.RCScheduler.Jobs.Utilities;
using Saras.SystemFramework.Core.Logging;

using static Saras.eMarking.RCScheduler.Jobs.Utilities.GenericApiCallHandler;

namespace Saras.eMarking.RCScheduler.Jobs.BusinessLogic
{
    public static class QRLpackStaticsManager
    {
        static readonly SarasLogger Log = new SarasLogger(typeof(UserSyncManager));

        public static string CallQRLpackStatics(string triggerName, string schedulerName)
        {
            string ApiResponse = string.Empty;
            string ApiUrl = string.Empty;
            int processStatus = 0;
            try
            {
                ApiUrl = string.Concat(ConfigurationManager.AppSettings.Get("UserSyncApiUrl"), "api/eMarking/eMarkingQRLpackStatics");
                ApiResponse = CallRestApi<string, string>(ApiUrl, string.Empty, HttpMethodType.Post);
                processStatus = 1;
                return ApiResponse;
            }
            catch (Exception ex)
            {
                processStatus = 2;
                Log.LogError(string.Format("Error in QRLpackStaticsManager CallQRLpackStatics Url : {0} and Exception Message : {1}", ApiUrl, ex.Message), ex);
            }
            finally
            {
                RcCheckDA rcCheckDA = new RcCheckDA();
                rcCheckDA.RcJobTrackingHistory(schedulerName, triggerName, triggerName, triggerName, 0, 0, 0, 0, 0, DateTime.UtcNow, null, ApiResponse, processStatus);
            }
            return ApiResponse;
        }
    }
}
