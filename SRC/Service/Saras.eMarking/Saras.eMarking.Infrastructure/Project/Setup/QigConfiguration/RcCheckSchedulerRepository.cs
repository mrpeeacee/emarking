using Microsoft.Extensions.Logging;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.ViewModels;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using Saras.eMarking.RCScheduler.Jobs.Factory;
using Saras.eMarking.RCScheduler.Jobs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Saras.eMarking.Infrastructure.Project.Setup.QigConfiguration
{
    public static class RcCheckSchedulerRepository
    {

        /// <summary>
        /// CreateUpdateRcSchedulerJob : This Method is used to create or update the RcScheduler Job
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        /// <param name="ProjectId"></param>
        /// <param name="QigId"></param>
        /// <param name="AppCache"></param>
        /// <param name="ProjectUserRoleId"></param>
        /// <param name="appSettingModels"></param>
        /// <param name="IsDelete"></param>
        /// <returns></returns>
        public static Task CreateUpdateRcSchedulerJob(ApplicationDbContext context, ILogger logger, long ProjectId, long QigId, IAppCache AppCache, long ProjectUserRoleId, List<AppSettingModel> appSettingModels = null, bool IsDelete = false)
        {
            try
            {
                List<RcDetails> ltrcDetails = new();

                List<AppSettingModel> rc_details = AppSettingRepository.GetAllSettings(context, logger, ProjectId, "RCSTNG", (int)EnumAppSettingEntityType.QIG, QigId).Result.ToList();

                if (rc_details.Count > 0)
                {
                    var rc1data = rc_details.FirstOrDefault(x => x.AppSettingKeyID == AppCache.GetAppsettingKeyId(EnumAppSettingKey.RandomCheckTier1));
                    var rc2data = rc_details.FirstOrDefault(x => x.AppSettingKeyID == AppCache.GetAppsettingKeyId(EnumAppSettingKey.RandomCheckTier2));
                    if (rc1data != null && Convert.ToBoolean(rc1data.Value) || rc2data != null && Convert.ToBoolean(rc2data.Value))
                    {
                        ltrcDetails.Add(new RcDetails
                        {
                            NewTmValue = appSettingModels?.FirstOrDefault(ap => ap.AppsettingKey == "JBTMT1")?.Value,
                            NewSmplValue = appSettingModels?.FirstOrDefault(ap => ap.AppsettingKey == "SMPLRTT1")?.Value,
                            RcType = 1,
                            TimeDuration = rc_details.Where(x => x.AppSettingKeyID == AppCache.GetAppsettingKeyId(EnumAppSettingKey.JobTimeTier1)).Select(x => x.Value).FirstOrDefault(),
                            SamplingRate = rc_details.Where(x => x.AppSettingKeyID == AppCache.GetAppsettingKeyId(EnumAppSettingKey.SampleRateTier1)).Select(x => x.Value).FirstOrDefault()
                        });
                    }
                    if (rc2data != null && Convert.ToBoolean(rc2data.Value))
                    {
                        ltrcDetails.Add(new RcDetails
                        {
                            NewTmValue = appSettingModels?.FirstOrDefault(ap => ap.AppsettingKey == "JBTMT2")?.Value,
                            NewSmplValue = appSettingModels?.FirstOrDefault(ap => ap.AppsettingKey == "SMPLRTT2")?.Value,
                            RcType = 2,
                            TimeDuration = rc_details.Where(x => x.AppSettingKeyID == AppCache.GetAppsettingKeyId(EnumAppSettingKey.JobTimeTier2)).Select(x => x.Value).FirstOrDefault(),
                            SamplingRate = rc_details.Where(x => x.AppSettingKeyID == AppCache.GetAppsettingKeyId(EnumAppSettingKey.SampleRateTier2)).Select(x => x.Value).FirstOrDefault()
                        });
                    }
                    if (ltrcDetails != null && ltrcDetails.Count > 0)
                    {
                        MarkingQuartzScheduler markingQuartzScheduler = new(context.Database.GetDbConnection().ConnectionString);

                        List<MarkingQuartzJob> markingQuartzJobs = GetRcsDetails(ltrcDetails, ProjectId, QigId, context, ProjectUserRoleId);

                        context.SaveChanges();
                        markingQuartzJobs.ForEach(async job =>
                        {
                            await markingQuartzScheduler.CreateJobAsync(job, logger, IsDelete);
                        });

                    }

                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// GetRcsDetails : This Method used get the Rc details
        /// </summary>
        /// <param name="ltrcDetails"></param>
        /// <param name="ProjectId"></param>
        /// <param name="QigId"></param>
        /// <param name="context"></param>
        /// <param name="ProjectUserRoleId"></param>
        /// <returns></returns>
        private static List<MarkingQuartzJob> GetRcsDetails(List<RcDetails> ltrcDetails, long ProjectId, long QigId, ApplicationDbContext context, long ProjectUserRoleId)
        {
            List<MarkingQuartzJob> markingQuartzJobs = new();
            foreach (var rc in ltrcDetails)
            {
                if (rc.NewTmValue == null || rc.NewTmValue != rc.TimeDuration)
                {
                    if (rc.NewTmValue != null)
                    {
                        rc.TimeDuration = rc.NewTmValue;
                    }
                    if (rc.NewSmplValue != null)
                    {
                        rc.SamplingRate = rc.NewSmplValue;
                    }
                    MarkingQuartzJob markingQuartzJob = new()
                    {
                        Name = "RCCheckJob_" + ProjectId + "_" + QigId + "_" + rc.RcType,
                        Group = "RCCheckJob_" + ProjectId + "_" + QigId + "_" + rc.RcType,
                        Description = "RCCheckJob_" + ProjectId + "_" + QigId + "_" + rc.RcType,
                        IsDurable = true,
                        Trigger = new MarkingQuartzTrigger
                        {
                            Name = "RCCheckJob_" + ProjectId + "_" + QigId + "_" + rc.RcType + "_" + rc.SamplingRate + "_" + rc.TimeDuration,
                            CronExpression = "0 */" + rc.TimeDuration + " * ? * *", //"0/" + rc.TimeDuration + " 0 0 ? * * *",// "0 0/" + rc.TimeDuration + " * * * ?",
                            Description = "RCCheckJob_" + ProjectId + "_" + QigId + "_" + rc.RcType + "_" + rc.SamplingRate + "_" + rc.TimeDuration,
                            Group = "RCCheckJob_" + ProjectId + "_" + QigId + "_" + rc.RcType + "_" + rc.SamplingRate + "_" + rc.TimeDuration,
                            Interval = Convert.ToInt32(rc.TimeDuration)
                        }
                    };
                    context.QrtzScheduleHistories.Add(new QrtzScheduleHistory
                    {
                        JobName = markingQuartzJob.Name,
                        TriggerName = markingQuartzJob.Trigger.Name,
                        JobGroup = markingQuartzJob.Group,
                        SchedName = "Saras.Scheduler.RcCheck",
                        TriggerGroup = markingQuartzJob.Trigger.Group,
                        CreatedBy = ProjectUserRoleId,
                        CreatedDate = DateTime.UtcNow,
                        RepeatInterval = Convert.ToInt32(rc.TimeDuration),
                        ProjectId = ProjectId,
                        QigId = QigId,
                        RcType = rc.RcType
                    });
                    markingQuartzJobs.Add(markingQuartzJob);
                }
            }
            return markingQuartzJobs;
        }
    }
}
