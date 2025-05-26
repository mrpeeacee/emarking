using Quartz;
using System;

namespace Saras.eMarking.RCScheduler.Jobs
{
    public  class RcInitializer : BaseJobScheduleManager, IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Log.LogInfo(string.Format("Job Details {0}", context.JobDetail));
                Quartz.Impl.JobDetailImpl jobDetailImpl = (Quartz.Impl.JobDetailImpl)context.JobDetail;
                string JobName = jobDetailImpl.Name;
                Log.LogInfo(string.Format("Executing Test Job Start Date Time : {0}", DateTime.UtcNow.ToString()));
                Log.LogInfo(string.Format("Job Schedule Name and details executed {0}", JobName));
            }
            catch(Exception ex)
            {
                Log.LogError(string.Format("Error while executing Rc Initializer job Method Method: Exception Message {0} context{1} ", ex.Message, context), ex);
            }
        }
    }
}
