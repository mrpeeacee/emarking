using Saras.eMarking.Domain.Extensions;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels.Project.Standardisation
{
    public class S2S3AssessmentModel
    {
        public S2S3AssessmentModel()
        {
            Scripts = new List<StandardisationScript>();
        }
        [XssTextValidation]
        public string Qigname { get; set; }
        public int Noofscripts { get; set; }
        public int Markedscript { get; set; }
        /// <summary>
        /// 1. Not Started
        /// 2. In Progress
        /// 3. Pass
        /// 4. Fail
        /// 5. Pending
        /// </summary>
        public int Result { get; set; }
        public string markingPersonal { get; set; } 
        public long? UserScriptMarkingRefID { get; set; }
        public decimal? TotalMarks { get; set; }
        public int ProcessStatus { get; set; }
        public int WorkflowId { get; set; }
        public int ApprovalStatus { get; set; }
        public string Remarks { get; set; }
        public string PauseRemarks { get; set; }
        public bool? IsAdditionalDone { get; set; }
        public bool IsQigPaused { get; set; }
        public List<StandardisationScript> Scripts { get; set; }
    }

    public class StandardisationScript
    {
        [XssTextValidation]
        public string ScriptName { get; set; }
        public bool? IsCompleted { get; set; }
        public bool? Result { get; set; }
        public long MarkedBy { get; set; }
        public long ScriptId { get; set; }
        public int WorkflowStatusID { get; set; }
        public long? UserMarkedBy { get; set; }
        public bool IsRecommended { get; set; }
        public bool? IsOutOfTolerance { get; set; }
        public long? RecommendedBy { get; set; }
        public decimal? version { get; set; }
        public long? UserScriptMarkingRefID { get; set; }
        public decimal? StdScore { get; set; }
        public decimal? MarkerScore { get; set; }
        public long? MScripId { get; set; }
        public long StdSheduleId { get; set; }
        public decimal? Stdcount { get; set; }
        public long? OrderIndex { get; set; }
        public long? QualifyingAssessmentId { get; set; }
    }

    public class AssignAdditionalStdScriptsModel
    {
        public long QIGID { get; set; }
        public long ProjectUserRoleID { get; set; }
        public List<AdditionalStdScriptsModel> ScriptIDs { get; set; }
    }
    public class AdditionalStdScriptsModel
    {
        public long ScriptId { get; set; }
        public string ScriptName { get; set; }
        public decimal? FinalizedMarks { get; set; }
        public bool IsSelected { get; set; }
        public long? UserScriptMarkingRefId { get; set; }
        public long? UserMarkedBy { get; set; }
        public bool IsCompleted { get; set; }
        public decimal? Version { get; set; }
    }
}
