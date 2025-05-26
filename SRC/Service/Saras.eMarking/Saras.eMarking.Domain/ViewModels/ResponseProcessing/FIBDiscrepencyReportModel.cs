using Saras.eMarking.Domain.Extensions;
using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels.ResponseProcessing
{

    public class FibDiscrepencyReportModel
    {
        public FibDiscrepencyReportModel()
        {

        }

        public long TotalNoOfScripts { get; set; }
        public long NoOfUnMarkedScripts { get; set; }
        public long NoOfMarkedScripts { get; set; }
        public long DistinctMarks { get; set; }
        [XssTextValidation]
        public string ResponseText { get; set; }
        public decimal? NormalisedScore { get; set; }
        public decimal? QuestionMarks { get; set; }
        public long? DiscrepancyStatus { get; set; }
        public long? MarkingType { get; set; }

        public List<FibDiscrepancy> FibDiscrepencies { get; set; }

        public List<MarkerDetails> FibMarkerDetails { get; set; }
    }

    public class FibDiscrepancy
    {
        public long SlNo { get; set; }
        public decimal? MarksAwarded { get; set; }
        public long NoOfMarkers { get; set; }
    }


    public class MarkerDetails
    {
        public string UserName { get; set; }
        public decimal? MarksAwarded { get; set; }
        public string LoginID { get; set; }
        public DateTime? MarkedDate { get; set; }
        public string ScriptName { get; set; }
        public long? Phase { get; set; }
        public long? ScriptID { get; set; }
        

    }
}
