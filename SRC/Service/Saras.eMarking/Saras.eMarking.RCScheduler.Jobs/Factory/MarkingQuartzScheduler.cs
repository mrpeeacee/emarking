using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using System.Collections.Specialized;

namespace Saras.eMarking.RCScheduler.Jobs.Factory
{
    public class MarkingQuartzScheduler
    {
        private readonly IScheduler _scheduler;
        private readonly string _connection = string.Empty;
        private MarkingQuartzScheduler()
        {
            _scheduler = GetScheduler().Result;

        }
        public MarkingQuartzScheduler(string ConnectionString)
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new ArgumentNullException(nameof(ConnectionString));
            }
            else
            {
                _connection = ConnectionString;
                _scheduler = GetScheduler().Result;
            }
        }
        public async Task<bool> CreateJobAsync(MarkingQuartzJob QuartzJob, ILogger logger, bool IsDelete = false)
        {
            bool status = false;
            try
            {
                if (!string.IsNullOrEmpty(QuartzJob.Name) && !string.IsNullOrEmpty(QuartzJob.Group) && !string.IsNullOrEmpty(QuartzJob?.Trigger?.Name) && !string.IsNullOrEmpty(QuartzJob?.Trigger?.Group))
                {
                    var jobKey = new JobKey(QuartzJob.Name, QuartzJob.Group);

                    var job = _scheduler.GetJobDetail(jobKey).Result;

                    ITrigger t;

                    if (job != null)
                    {
                        //Clean out old jobs - just for this sample. In production we will most likely update the job 
                        await _scheduler.DeleteJob(jobKey);
                    }
                    if (!IsDelete)
                    {
                        job = JobBuilder.Create(typeof(RcCheckScheduler))
                                   .WithIdentity(jobKey)
                                   .RequestRecovery()
                                   .StoreDurably(QuartzJob.IsDurable)
                                   .Build();

                        t = (ISimpleTrigger)TriggerBuilder.Create()
                                                                 .ForJob(job)
                                                                 .WithIdentity(QuartzJob.Trigger.Name, QuartzJob.Trigger.Group)
                                                                 .StartAt(DateTimeOffset.UtcNow.AddMinutes(QuartzJob.Trigger.Interval))
                                                                 .WithSimpleSchedule(x => x.WithIntervalInMinutes(QuartzJob.Trigger.Interval).RepeatForever())
                                                                 .Build();


                        await _scheduler.ScheduleJob(job, t);
                    }
                    status = true;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error in MarkingQuartzScheduler >> CreateJobAsync : Method Name: CreateJobAsync()");
                throw;
            }
            return status;
        }


        private async Task<IScheduler> GetScheduler()
        {
            var properties = new NameValueCollection
            {
                { "quartz.scheduler.instanceName", "Saras.Scheduler.RcCheck" },
                { "quartz.scheduler.instanceId", "Saras.Scheduler" },
                { "quartz.jobStore.type", "Quartz.Impl.AdoJobStore.JobStoreTX, Quartz" },
                { "quartz.jobStore.useProperties", "true" },
                { "quartz.jobStore.dataSource", "default" },
                { "quartz.jobStore.tablePrefix", "Quartz.QRTZ_" },
                {
                    "quartz.dataSource.default.connectionString",
                    _connection
                },
                { "quartz.dataSource.default.provider", "SqlServer" },
                { "quartz.threadPool.threadCount", "1" },
                { "quartz.serializer.type", "json" },
            };
            StdSchedulerFactory? schedulerFactory = new(properties);

            return await schedulerFactory.GetScheduler();
        }
    }
}
