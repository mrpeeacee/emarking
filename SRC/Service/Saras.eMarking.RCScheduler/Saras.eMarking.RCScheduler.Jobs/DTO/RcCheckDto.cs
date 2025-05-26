using System;
using System.Collections.Generic;

namespace Saras.eMarking.RCScheduler.Jobs.DTO
{
    public class RcScripts
    {
        public long PhaseStatusTrackingID { get; set; }
        public long ProjectID { get; set; }
        public long Qigid { get; set; }
        public long ScriptID { get; set; }
        public long ActionBy { get; set; }
        public bool IsSelectedForRC { get; set; }
        public bool IsJobRun { get; set; }
    }

    public class RandomCheck
    {
        public decimal SamplingRate { get; set; }

        public List<RcScripts> Scripts { get; set; }
    }
}
