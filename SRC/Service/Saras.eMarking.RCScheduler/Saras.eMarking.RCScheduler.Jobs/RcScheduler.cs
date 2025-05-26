using System;
using Quartz;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Saras.eMarking.RCScheduler.Jobs.BusinessLogic;

namespace Saras.eMarking.RCScheduler.Jobs
{
    public class RcCheckScheduler : BaseJobScheduleManager, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                string statusMessage = string.Empty;
                //Quartz.Impl.JobDetailImpl jobDetailImpl = (Quartz.Impl.JobDetailImpl)context.JobDetail
                string triggerName = ((Quartz.Impl.Triggers.AbstractTrigger)context.Trigger).Name;
                Log.LogInfo(string.Format("Business Logic Rc Check Scheduler Method Start Date Time : {0}, Trigger Name : {1}", DateTime.UtcNow.ToString(), triggerName));

                statusMessage = new RcCheck().GetAndMoveScriptsToRcPool(triggerName, context.Scheduler.SchedulerName);
                Log.LogInfo(string.Format("Successfully executed the Business Logic Rc Check Scheduler Method End Date Time : {0} , Trigger Name : {1}", DateTime.UtcNow.ToString(), triggerName));
                Log.LogInfo(string.Format("Returned Status Message {0}", statusMessage));
            }
            catch (Exception ex)
            {
                Log.LogError(string.Format("Error while executing BusinessLogic LC Rc Check Scheduler Method: Exception Message {0} context{1} ", ex.Message, context), ex);
            }
        }
    }
}
