using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Quartz;

using Saras.eMarking.RCScheduler.Jobs.BusinessLogic;

namespace Saras.eMarking.RCScheduler.Jobs
{
    public class SendEmailScheduler : BaseJobScheduleManager, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                string EmailstatusMessage = string.Empty;
                Log.LogInfo(string.Format("CallSendEmail Method Started. Call API Date Time : {0} ", DateTime.UtcNow.ToString()));
                string triggerName = ((Quartz.Impl.Triggers.AbstractTrigger)context.Trigger).Name;
                EmailstatusMessage = SendEmailManager.CallSendEmail(triggerName, context.Scheduler.SchedulerName);
                Log.LogInfo(string.Format("CallSendEmail Method Started Successfully and called API and End Date Time : {0}", DateTime.UtcNow.ToString()));
                Log.LogInfo(string.Format("CallSendEmail Method Returned Status Message {0}", EmailstatusMessage));
            }
            catch (Exception ex)
            {
                Log.LogError(string.Format("Error while calling CallSendEmail API Exception Message {0} context{1} ", ex.Message, context), ex);
            }
        }
    }
}
