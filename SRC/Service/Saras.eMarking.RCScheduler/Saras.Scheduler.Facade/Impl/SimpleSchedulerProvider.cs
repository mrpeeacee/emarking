using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using Saras.Scheduler.Facade.Config;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.AdoJobStore;
using Saras.SystemFramework.Core.Configuration;

namespace Saras.Scheduler.Facade.Impl
{
    class SimpleSchedulerProvider : ISchedulerProvider
    {
        #region ISchedulerProvider Members

        public IScheduler GetScheduler()
        {

            SchedulerSettings section = (SchedulerSettings)ConfigurationManager.GetSection(SchedulerSettings.SectionName);

            if (section == null)
                throw new ConfigurationErrorsException(string.Format("'{0}' configuration section was not found or is misconfigured", SchedulerSettings.SectionName));

            NameValueCollection customSchedulerSettings = new NameValueCollection();
            customSchedulerSettings.Add(StdSchedulerFactory.PropertySchedulerInstanceName, "Saras.Scheduler.RcCheck");

            if (section.Settings.IsClustered)
                customSchedulerSettings.Add(StdSchedulerFactory.PropertySchedulerInstanceId, StdSchedulerFactory.AutoGenerateInstanceId);
            else
                customSchedulerSettings.Add(StdSchedulerFactory.PropertySchedulerInstanceId, "Saras.Scheduler");

            customSchedulerSettings.Add(StdSchedulerFactory.PropertySchedulerIdleWaitTime, section.Settings.IdleWaitTime.ToString());
            customSchedulerSettings.Add(StdSchedulerFactory.PropertySchedulerDbFailureRetryInterval, section.Settings.DbFailureRetryInterval.ToString());
            customSchedulerSettings.Add(StdSchedulerFactory.PropertyThreadPoolPrefix + ".threadCount", section.Settings.ThreadCount.ToString());
            customSchedulerSettings.Add(StdSchedulerFactory.PropertyJobStorePrefix + ".misfireThreshold", section.Settings.MisfireThreshold.ToString());
            customSchedulerSettings.Add(StdSchedulerFactory.PropertyJobStoreType, typeof(JobStoreTX).AssemblyQualifiedName);

            customSchedulerSettings.Add(StdSchedulerFactory.PropertyJobStorePrefix + ".driverDelegateType", typeof(SqlServerDelegate).AssemblyQualifiedName);
            customSchedulerSettings.Add(StdSchedulerFactory.PropertyJobStorePrefix + ".useProperties", "false");
            customSchedulerSettings.Add(StdSchedulerFactory.PropertyJobStorePrefix + ".tablePrefix", section.Settings.TablePrefix);
            customSchedulerSettings.Add(StdSchedulerFactory.PropertyJobStorePrefix + ".dataSource", "default");
            customSchedulerSettings.Add(StdSchedulerFactory.PropertyJobStorePrefix + ".clustered", section.Settings.IsClustered.ToString().ToLower());
            customSchedulerSettings.Add(StdSchedulerFactory.PropertyJobStorePrefix + ".selectWithLockSQL", "SELECT * FROM {0}LOCKS UPDLOCK WHERE LOCK_NAME = @lockName");
            customSchedulerSettings.Add(StdSchedulerFactory.PropertyDataSourcePrefix + ".default.provider", "SqlServer-20");
             
            customSchedulerSettings.Add(StdSchedulerFactory.PropertyDataSourcePrefix + ".default.connectionString", ConfigurationManager.ConnectionStrings["SchedulerDB"].ConnectionString);

            return new StdSchedulerFactory(customSchedulerSettings).GetScheduler();
        }

        #endregion
    }
}

namespace Saras.Scheduler.Facade.Jobs
{
    public interface ISampleJob : Quartz.IJob
    {

    }
}