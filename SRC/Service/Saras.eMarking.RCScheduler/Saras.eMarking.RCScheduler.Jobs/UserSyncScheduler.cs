using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Quartz;

using Saras.eMarking.RCScheduler.Jobs.BusinessLogic;

namespace Saras.eMarking.RCScheduler.Jobs
{
    public class UserSyncScheduler : BaseJobScheduleManager, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                string statusMessage = string.Empty;
                Log.LogInfo(string.Format("UserSyncScheduler Method Started. Call API Date Time : {0} ", DateTime.UtcNow.ToString()));
                string triggerName = ((Quartz.Impl.Triggers.AbstractTrigger)context.Trigger).Name;
                statusMessage = UserSyncManager.CallUserSync(triggerName, context.Scheduler.SchedulerName);
                Log.LogInfo(string.Format("UserSyncScheduler Method Started Successfully and called API and End Date Time : {0}", DateTime.UtcNow.ToString()));
                Log.LogInfo(string.Format("UserSyncScheduler Method Returned Status Message {0}", statusMessage));
            }
            catch (Exception ex)
            {
                Log.LogError(string.Format("Error while calling User Sync API Exception Message {0} context{1} ", ex.Message, context), ex);
            }
        }
    }
}
