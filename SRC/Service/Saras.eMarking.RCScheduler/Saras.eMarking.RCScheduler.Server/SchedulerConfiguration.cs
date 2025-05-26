using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;
using Saras.SystemFramework.Core.Configuration;

namespace Saras.eMarking.RCScheduler.Server
{
   
    public class SchedulerConfiguration
    {
        private const string PrefixServerConfiguration = "quartz.server";
        private const string KeyServiceName = PrefixServerConfiguration + ".serviceName";
        private const string KeyServiceDisplayName = PrefixServerConfiguration + ".serviceDisplayName";
        private const string KeyServiceDescription = PrefixServerConfiguration + ".serviceDescription";
        private const string KeyServerImplementationType = PrefixServerConfiguration + ".type";

        private const string DefaultServiceName = "QuartzServer";
        private const string DefaultServiceDisplayName = "Quartz Server";
        private const string DefaultServiceDescription = "Quartz Job Scheduling Server";
        

        private static SchedulerSettings section = null;

        /// <summary>
        /// Initializes the <see cref="Configuration"/> class.
        /// </summary>
        static SchedulerConfiguration()
        {
            section= (SchedulerSettings)ConfigurationManager.GetSection(SchedulerSettings.SectionName);
        }

        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        /// <value>The name of the service.</value>
        public static string ServiceName
        {
            get { return section.Settings.ServiceName; }
        }

        /// <summary>
        /// Gets the display name of the service.
        /// </summary>
        /// <value>The display name of the service.</value>
        public static string ServiceDisplayName
        {
            get { return section.Settings.ServiceName; }
        }

        /// <summary>
        /// Gets the service description.
        /// </summary>
        /// <value>The service description.</value>
        public static string ServiceDescription
        {
            get { return section.Settings.ServiceName; }
        }


        /// Gets the service description.
        /// </summary>
        /// <value>The service description.</value>
        public static string ServiceJobFilePath
        {
            get { return section.Settings.IntialJobFile; }
        }

        /// Gets the service description.
        /// </summary>
        /// <value>The service description.</value>
        public static bool  InsertJobs
        {
            get { return section.Settings.InsertJobs; }
        }


    }
}
