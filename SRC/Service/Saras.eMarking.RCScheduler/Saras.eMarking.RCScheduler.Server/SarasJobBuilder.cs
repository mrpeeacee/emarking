using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;
using Quartz;


namespace Saras.eMarking.RCScheduler.Server
{
    public class SarasJobBuilder
    {
        public static void LoadInitialJobData()
        {
            //const string OPTION_ADD = "1"
            //const string OPTION_DELETE = "2"
            const string NODES_JOBS_EXPRE = "quartz/job/job-detail";
            const string NODE_NAME = "name";
            const string NODE_GROUP = "group";
            const string NODE_JOBTYPE = "job-type";
            const string NODE_TIMEINTERVAL = "TimeInterval";
            const string NODE_DURABLE = "durable";
            const string NODE_RECOVER = "recover";

            //const string TRIGGER_RECOVER = "trigger"
            const string NODE_TRIGGERTYPE = "TriggerType";
            const string NODE_TRIGGERHR = "TriggerHr";
            const string NODE_TRIGGERMIN = "TriggerMin";

            if (SchedulerConfiguration.InsertJobs)
            {
                var sched = Saras.Scheduler.Facade.SchedulerProviderFactory.Provider.GetScheduler();

                //ClearJobs(sched);

                string JobXMLPath = string.Concat(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), SchedulerConfiguration.ServiceJobFilePath);
                if (!System.IO.File.Exists(JobXMLPath))
                {
                    Console.WriteLine(string.Format("Jobs Installer Descriptor filename {0} should be placed in the working directory of the JobInstaller executable. ErrorCode: {1}", JobXMLPath, ExitCodes.JOBS_XML_MISSING));
                    Environment.Exit((int)ExitCodes.JOBS_XML_MISSING);
                }

                // Read the XML document
                XPathDocument myXPathDocument = new XPathDocument(JobXMLPath);

                AddJobs(sched, NODES_JOBS_EXPRE, NODE_NAME, NODE_GROUP, NODE_JOBTYPE, NODE_TIMEINTERVAL, NODE_DURABLE, NODE_RECOVER, NODE_TRIGGERTYPE, NODE_TRIGGERHR, NODE_TRIGGERMIN, myXPathDocument);
            }
        }


        public static void ClearJobs(IScheduler inScheduler)
        {
            inScheduler.Clear();
        }

        public static void AddJobs(IScheduler sched, string NODES_JOBS_EXPRE, string NODE_NAME, string NODE_GROUP, string NODE_JOBTYPE, string NODE_TIMEINTERVAL, string NODE_DURABLE, string NODE_RECOVER, string NODE_TRIGGERTYPE, string NODE_TRIGGERHR, string NODE_TRIGGERMIN, XPathDocument myXPathDocument)
        {
            XPathNavigator myXPathNavigator = myXPathDocument.CreateNavigator();
            XPathNodeIterator nodes_job = myXPathNavigator.Select(NODES_JOBS_EXPRE);

            while (nodes_job.MoveNext())
            {
                XPathNavigator jname = nodes_job.Current.SelectSingleNode(NODE_NAME);
                XPathNavigator jgname = nodes_job.Current.SelectSingleNode(NODE_GROUP);
                XPathNavigator jtype = nodes_job.Current.SelectSingleNode(NODE_JOBTYPE);
                XPathNavigator jRequestsRecovery = nodes_job.Current.SelectSingleNode(NODE_RECOVER);
                XPathNavigator jDurable = nodes_job.Current.SelectSingleNode(NODE_DURABLE);

                XPathNodeIterator nodes_triggerr = myXPathNavigator.Select("quartz/job/trigger[@id='" + jname.InnerXml + "']/simple");
                int counter = 0;
                int triggerPriority = 20;

                //while loops is introduced to create multiple triggers for the same job.
                while (nodes_triggerr.MoveNext())
                {
                    XPathNavigator nodes_trigger = nodes_triggerr.Current;

                    if (nodes_trigger.IsNode)
                    {
                        XPathNavigator tname = nodes_trigger.SelectSingleNode(NODE_NAME);
                        XPathNavigator tgname = nodes_trigger.SelectSingleNode(NODE_GROUP);
                        XPathNavigator ttype = nodes_trigger.SelectSingleNode(NODE_TRIGGERTYPE);
                        try
                        {
                            string jobName = jname.InnerXml;
                            string jobGroupName = jgname.InnerXml;
                            string jobType = jtype.InnerXml;
                            bool jobRequestsRecovery = System.Convert.ToBoolean(jRequestsRecovery.InnerXml);
                            bool jobDurable = System.Convert.ToBoolean(jDurable.InnerXml);
                            string triggerName = tname.InnerXml;
                            string triggerGroupName = tgname.InnerXml;
                            string triggertype;

                            if (ttype == null) { triggertype = "1"; } else { triggertype = ttype.InnerXml; } //Default Minutly trigger will be created

                            var jobKey = new JobKey(jobName, jobGroupName);

                            var job = sched.GetJobDetail(jobKey);

                            bool jobIsNew = (null == job);

                            var triggerKey = new TriggerKey(triggerName, triggerGroupName);

                            if (!jobIsNew)
                            {
                                //Clean out old jobs - just for this sample. In production we will most likely update the job
                                sched.UnscheduleJob(triggerKey);
                            }

                            job = JobBuilder.Create(Type.GetType(jobType))
                                       .WithIdentity(jobKey)
                                       .RequestRecovery(jobRequestsRecovery)
                                       .StoreDurably(jobDurable)
                                       .Build();

                            ITrigger t = null;

                            if (triggertype == "1")
                            {
                                XPathNavigator tinterval = nodes_trigger.SelectSingleNode(NODE_TIMEINTERVAL);

                                int triggerTimeInterval = 0;
                                if (!Int32.TryParse(tinterval.InnerXml, out triggerTimeInterval))
                                {
                                    triggerTimeInterval = 1;
                                }

                                t = (ISimpleTrigger)TriggerBuilder.Create()
                                                          .ForJob(job)
                                                          .WithIdentity(triggerName, triggerGroupName)
                                                          .StartAt(DateBuilder.TodayAt(0, 0, 0))  // Start at the beginning of the day
                                                          .WithSimpleSchedule(x => x.WithIntervalInMinutes(triggerTimeInterval).RepeatForever())
                                                          .WithPriority(triggerPriority)
                                                          .Build();

                            }
                            else
                            {
                                XPathNavigator thr = nodes_trigger.SelectSingleNode(NODE_TRIGGERHR);
                                int triggerHour = System.Convert.ToInt32(thr.InnerXml);
                                XPathNavigator tmin = nodes_trigger.SelectSingleNode(NODE_TRIGGERMIN);
                                int triggerMin = System.Convert.ToInt32(tmin.InnerXml);

                                t = TriggerBuilder.Create()
                                                         .ForJob(job)
                                                         .WithIdentity(triggerName, triggerGroupName)
                                                         .WithDailyTimeIntervalSchedule(x => x.OnEveryDay()
                                                         .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(triggerHour, triggerMin))
                                                         .WithIntervalInHours(24))  // Repeat every 24 hours
                                                         .WithPriority(triggerPriority)
                                                         .Build();
                            }

                            if (sched.CheckExists(jobKey))
                            {
                                sched.DeleteJob(jobKey);
                            }

                            if (counter == 0)
                            {
                                sched.ScheduleJob(job, t);
                            }
                            else
                            {
                                sched.ScheduleJob(t);
                            }
                            counter++;
                            triggerPriority = triggerPriority - 5;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            Console.WriteLine(Saras.eMarking.RCScheduler.Server.JobSchedulerResource.ResourceManager.GetString("InValidJOB") + jtype.InnerXml);
                        }
                    }
                    else
                    {
                        Console.WriteLine(Saras.eMarking.RCScheduler.Server.JobSchedulerResource.ResourceManager.GetString("InValidTrigger") + jtype.InnerXml);
                    }
                }
            }
        }
    }

    enum ExitCodes
    {
        JOBS_XML_MISSING = -1
    }
}
