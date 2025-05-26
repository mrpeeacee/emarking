using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Quartz;

using Saras.eMarking.RCScheduler.Jobs.BusinessLogic;

namespace Saras.eMarking.RCScheduler.Jobs
{
    public class QRLpackStaticsScheduler : BaseJobScheduleManager, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                string QLRstatusMessage = string.Empty;
                Log.LogInfo(string.Format("CallQRLpackStatics Method Started. Call API Date Time : {0} ", DateTime.UtcNow.ToString()));
                string triggerName = ((Quartz.Impl.Triggers.AbstractTrigger)context.Trigger).Name;
                QLRstatusMessage = QRLpackStaticsManager.CallQRLpackStatics(triggerName, context.Scheduler.SchedulerName);
                Log.LogInfo(string.Format("CallQRLpackStatics Method Started Successfully and called API and End Date Time : {0}", DateTime.UtcNow.ToString()));
                Log.LogInfo(string.Format("CallQRLpackStatics Method Returned Status Message {0}", QLRstatusMessage));
            }
            catch (Exception ex)
            {
                Log.LogError(string.Format("Error while calling CallQRLpackStatics API Exception Message {0} context{1} ", ex.Message, context), ex);
            }
        }
    }
}
