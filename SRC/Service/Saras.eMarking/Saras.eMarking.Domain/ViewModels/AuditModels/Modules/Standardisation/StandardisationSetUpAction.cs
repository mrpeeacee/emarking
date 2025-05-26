using Saras.eMarking.Domain.ViewModels.AuditTrail;
using Saras.eMarking.Domain.ViewModels.Project.MarkScheme;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels.AuditModels.Modules.Standardisation
{
    public class StandardisationSetUpAction
    {
        public long QigId { get; set; }
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        public bool? IsKP { get; set; }
    }

   

    public class ConfigurationAction
    {
        public long QigId { get; set; }
        public string RecommendationPoolCount { get; set; }
        public string RecommendationCountkp { get; set; }
    } 

    public class CategorisationAction
    {
        public long ScriptId { get; set; }
        public long QigId { get; set; }
        public long UserScriptMarkingRefId { get; set;}
        public string PoolType { get; set; }        
        public bool SelectAsDefinitive { get; set; }
    }

    public class QualifyingAssessmentAction
    {
        public long QIGId { get; set; }
        public long PresentationMode { get; set; }
        public long ApprovalType { get; set; }
        public long TolerenceCount { get; set; }
        public long TotalNoOfScripts { get; set; }
        public long NoOfScriptSelected { get; set; }
        public long ScriptCategorizationPoolId { get; set; }
    }

    public class  TrailMarkingAction
    {
        public  long? ScriptID { get; set; }
        public long? BandID { get; set; }
        public long? ScheduleUserId { get; set; }
        public long? MarkedBy { get; set; }
        public long? MarkingStatus { get; set; }
        public decimal? AwardedMarks { get; set; } 
        public long? ProjectQuestionResponseID { get; set; }
        public long? TotalMarks { get; set; }
        public long?  WorkflowstatusID { get; set; }
    }

    public class MarksSchemeAction : IAuditTrail
    {
        public string MarksSchemeName { get; set; }
        public long Max_Marks { get; set; }
        public string Description { get; set; }
        public List<BandModel> Bands { get; set; }
    }



    public class BandDetails : IAuditTrail
    {
        public string BandName { get; set; }
        public string BandCode { get; set; }
        public string BandDescription { get; set; }
        public decimal BandFrom { get; set; }
        public decimal BandTo { get; set; }

    }

    public class AllBandValues : IAuditTrail
    {
        public List<BandDetails> BandDetails { get; set; }
    }
    public class MarksSchemeDelete : IAuditTrail
    {
        public long DeletedId { get; set; }
    }

    public class MarksSchemeQuestionText : IAuditTrail
    {
        public long QuestiontextId { get; set; }
    }

    public class S1ClosureCompleted
    {
        public long EntityID { get; set; }
        public byte EntityType { get; set; }        
        public byte ProcessStatus { get; set; }
        public string WorkflowCode { get; set; }
    }
}
