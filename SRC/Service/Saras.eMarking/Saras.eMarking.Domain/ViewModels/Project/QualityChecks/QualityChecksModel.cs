using Saras.eMarking.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Project.QualityChecks
{
    public class QualityChecksModel
    {

        public long ProjectUserRoleID { get; set; }
        public string ProjectUserName { get; set; }
        public string RoleCode { get; set; }
        public long ReportingTo { get; set; }
        public bool IsKp { get; set; }
    }


    public class QualityCheckInitialScriptModel
    {
        public long ScriptId { get; set; }
        public bool CheckedOutByMe { get; set; }
        public bool CheckoutEnabled { get; set; }
        public int WorkflowStatusID { get; set; }
        public int RcLevel { get; set; }
        public bool IsInGracePeriod { get; set; }
        public bool IsRc2Adhoc { get; set; }
        public int Checkstatus { get; set; }
        public bool RettomarEnable { get; set; }
        public List<ScriptChildModel> ScriptChildModel { get; set; }
    }

    public class ScriptChildModel
    {
        public long PhaseStatusTrackingId { get; set; }
        public byte Phase { get; set; }
        public byte? Status { get; set; }
        public string MarkedBy { get; set; }
        public decimal? MarksAwarded { get; set; }
        public long? ActionBy { get; set; }
        public bool? IsActive { get; set; }
        public long? UserScriptMarkingRefId { get; set; }
        public string Remarks { get; set; }
        public DateTime? Submitted { get; set; }
        public bool IsRCJobRun { get; set; }
        public bool IsScriptFinalised { get; set; }
        public int RcLevel { get; set; }
        public string RoleCode { get; set; }
    }


    public class LivemarkingApproveModel
    {
        public long PhaseStatusTrackingId { get; set; } 
        public long QigID { get; set; }
        public long ScriptID { get; set; }
        public long ProjectUserRoleID { get; set; }
        [XssTextValidation]
        [MaxLength(500)]
        public string Remark { get; set; }
        public int Status { get; set; }
        public bool IsCheckout { get; set; }
        public int WorkflowstatusId { get; set; } 
    }


    public class TrialmarkingScriptDetails
    {
        public long ProjectID { get; set; }
        public long QigID { get; set; }
        public long ScriptID { get; set; }
        public long ProjectUserRoleID { get; set; }
        public int WorkflowstatusId { get; set; }
    }


    public class QualityCheckScript
    {
        public List<QualityCheckScriptDetailsModel> QualityCheckScriptDetailsModel { get; set; }
        public QualityCheckCountSummary QualityCheckCountSummary { get; set; }
    }

    public class QualityCheckScriptDetailsModel
    {
        public long ScriptId { get; set; }
        public long ProjectUserRoleID { get; set; }
        public string ScriptName { get; set; }
        public long SampledRc1 { get; set; }
        public long RC1Done { get; set; }
        public long SampledRc2 { get; set; }
        public long RC2Done { get; set; }
        public long AdhocChecked { get; set; }
        public long Rowcount { get; set; }
        public int SubmittedPhaseByMe { get; set; }
        public bool IsScriptCheckedOut { get; set; }
        public string CheckedOutName { get; set; }
        public bool IsFinalised { get; set; }
        public string RoleName { get; set; }
        public DateTime? ACTIONDATE { get; set; }
        public Boolean IsLivePoolEnable { get; set; }
        public long PhaseStatusTrackingID { get; set; }
        public byte? Scriptstatus { get; set; }
        public byte? Phase { get; set; }
    }

    public class QualityCheckCountSummary
    {
        public long? Submitted { get; set; }
        public long? TotalScripts { get; set; }
        public long? Reallocated { get; set; }
        public long? ScriptRcdT1 { get; set; }
        public long? InGracePeriod { get; set; }
        public long? ScriptRcToBeT1 { get; set; }
        public long? ScriptRcToBeT2 { get; set; }
        public long? ScriptRcdT2 { get; set; }
        public long? RandomChecked { get; set; }
        public int RcLevel { get; set; }
        public long? AdhocChecked { get; set; }
        public long? Resubmitted { get; set; }
        public long? Downloaded { get; set; }
        public long? Returntomarker { get; set; }
    }

    public class Qualitycheckedbyusers
    {
        public string ScriptName { get; set; }
        public long? ProjectUserRoleID { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; }
        public long ScriptId { get; set; }

    }

    public class TeamStatistics
    {
       public long QigId { get; set; }
       public long ProjectId { get; set; }
       public int Responsetype { get; set; }
       public int Filter { get; set; }
       public int CountOrDetails { get; set; }
       public long ProjectUserRoleID { get; set; }
    }

}
