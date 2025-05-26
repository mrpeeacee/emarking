using System;
using System.Collections.Generic;
using System.Text;
using Quartz;

namespace Saras.Scheduler.Facade
{
    public interface ISchedulerProvider
    {
        IScheduler GetScheduler( );
    }
}
