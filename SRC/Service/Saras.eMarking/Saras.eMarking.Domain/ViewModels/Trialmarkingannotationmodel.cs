using System;
using System.Collections.Generic;
using Saras.eMarking.Domain.ViewModels.Project.MarkScheme;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Saras.eMarking.Domain.ViewModels
{
    public class Trialmarkingannotationmodel
    {

        public string Path { get; set; }
        public string AnnotationToolName { get; set; }
        public string AnnotationGroupName { get; set; }
        public Nullable<decimal> AssociatedMark { get; set; }
    }
    public class DownloadMarkschemeModel
    {
        public MarkSchemeType MarkschemeType { get; set; }
        public List<MarkSchemeModel> MarkSchemes { get; set; }
       
    }
    public class ViewScriptModel
    {
        public long Type { get; set; }
        public long ScriptID { get; set; }
        public string LoginName { get; set; }
        public string ScriptName { get; set; }
        public string QIGName { get; set; }
        public long? WorkFlowStatusID { get; set; }
        public long? MarkedBy { get; set; }
        public string MarkerName { get; set; }
        public decimal? MarksAwarded { get; set; }
        public DateTime? MarkedDate { get; set; }
        public long? ScriptPhaseTrackingID { get; set; }
        public long? UserScriptMarkingRefID { get; set; }
        public int Phase { get; set; }
        public string RoleCode { get; set; }
        public bool SelectAsDefinitive { get; set; }
    }

}

