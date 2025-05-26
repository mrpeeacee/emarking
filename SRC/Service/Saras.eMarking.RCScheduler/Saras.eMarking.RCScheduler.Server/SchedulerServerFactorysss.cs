using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Topshelf;
using Quartz;
using Quartz.Impl;
using log4net;

namespace Saras.eMarking.RCScheduler.Server
{
    public class SchedulerServerFactory
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SchedulerServerFactory));

        /// <summary>
        /// Creates a new instance of an Quartz.NET server core.
        /// </summary>
        /// <returns></returns>
        public static ISchedulerServer CreateServer()
        {
            
            string typeName = typeof(SarasSchedulerServer).AssemblyQualifiedName;

            Type t = Type.GetType(typeName, true);

            logger.Debug("Creating new instance of server type '" + typeName + "'");
            
            ISchedulerServer retValue = (ISchedulerServer)Activator.CreateInstance(t);
           
            logger.Debug("Instance successfully created");
            
            return retValue;
        }
    }   
}
