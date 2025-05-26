using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Topshelf;
using System.Configuration;
using Quartz;
using System.Threading;
using log4net;
using System.Xml.XPath;


namespace Saras.eMarking.RCScheduler.Server
{
    /// <summary>
    /// The server's main entry point.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main.
        /// </summary>
        public static void Main()
        {
            // change from service account's dir to more logical one
            Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);

            SarasJobBuilder.LoadInitialJobData();

            HostFactory.Run(x =>
            {
                x.RunAsLocalSystem();
                
                x.SetDescription(SchedulerConfiguration.ServiceDescription);
                x.SetDisplayName(SchedulerConfiguration.ServiceDisplayName);
                x.SetServiceName(SchedulerConfiguration.ServiceName);
                x.UseLog4Net("log4net.config");
                x.Service(factory =>
                {
                    SarasSchedulerServer server = new SarasSchedulerServer();
                    server.Initialize();
                    return server;
                });
            });
        }
    }
}
