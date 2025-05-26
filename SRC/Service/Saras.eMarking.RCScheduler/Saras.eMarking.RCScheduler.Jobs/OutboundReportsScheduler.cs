using Quartz;
using Saras.eMarking.RCScheduler.Jobs.BusinessLogic;
using System; 

namespace Saras.eMarking.RCScheduler.Jobs
{
    public class OutboundReportsScheduler : BaseJobScheduleManager, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                string EmailstatusMessage = string.Empty;
                Log.LogInfo(string.Format("OutboundReportsScheduler Method Started. Call API Date Time : {0} ", DateTime.UtcNow.ToString()));
                string triggerName = ((Quartz.Impl.Triggers.AbstractTrigger)context.Trigger).Name;
                EmailstatusMessage = OutboundReportsManager.SyncReport(triggerName, context.Scheduler.SchedulerName);
                Log.LogInfo(string.Format("OutboundReportsScheduler Method Started Successfully and called API and End Date Time : {0}", DateTime.UtcNow.ToString()));
                Log.LogInfo(string.Format("OutboundReportsScheduler Method Returned Status Message {0}", EmailstatusMessage));
            }
            catch (Exception ex)
            {
                Log.LogError(string.Format("Error while calling CallSendEmail API Exception Message {0} context{1} ", ex.Message, context), ex);
            }
        }
    }
}
