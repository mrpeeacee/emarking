using Saras.eMarking.Domain.ViewModels.AuditTrail;
using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration
{
    public class QigSummaryModel
    {
        
        public Boolean IsQigSetup { get; set; }
        public Boolean IsLiveMarkingStart { get; set; }
        public List<RcDetails> fd { get; set; }

    }



    public class RcDetails
    {
        public int RcType { get; set; }
        public string TimeDuration { get; set; }
        public string SamplingRate { get; set; }
        public string NewTmValue { get; set; }
        public string NewSmplValue { get; set; }
    }
    public class QigSummaryAction : IAuditTrail
    {
        public bool isProjectSetup { get; set; }

        public bool isLiveMarking { get; set; }

        public long ProjectUserRoleID { get; set; }

        public long ProjectID { get; set; }

        public long Qigid { get; set; }
    }
}
