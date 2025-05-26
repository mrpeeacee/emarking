using System;
using System.Collections.Generic;
using System.Text;

using Saras.Scheduler.Facade.Impl;

namespace Saras.Scheduler.Facade
{
    public static class SchedulerProviderFactory
    {
        public static ISchedulerProvider Provider
        {
            get
            {
                return new SimpleSchedulerProvider( );
            }
        }
    }

}
