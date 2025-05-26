using Quartz;

namespace Saras.eMarking.RCScheduler.Jobs
{
    public class RcCheckScheduler :  IJob
    {
#pragma warning disable IDE0060 // Remove unused parameter
        public static void Execute(IJobExecutionContext context)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            // Method intentionally left empty.
        }

        Task IJob.Execute(IJobExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}