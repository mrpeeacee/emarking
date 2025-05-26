using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels.Project.Standardisation.S2S3Approvals
{
    public class S2S3ApprovalsModel
    {
        public long ScriptCount { get; set; }
        public long? ToleranceCount { get; set; }
        public long? ApprovalType { get; set; }
        public long ProjectUserRoleID { get; set; }
        public string ProjectUserName { get; set; }
        public string RoleCode { get; set; }
        public long? ReportingTo { get; set; }
        public string ReportingToName { get; set; }
        public bool IsKp { get; set; }
    }
    public class MarkingPersonal
    {
        public long? UserRoleId { get; set; }
        public string MPName { get; set; }
        public string Role { get; set; }
        public string Supervisor { get; set; }
        public string SvsName { get;set; }
        public long? OutOfTolerance { get; set; }
        public long? S2S3AddScript { get; set; }
        public long? Result { get; set; }
        public string ApprovalStatus { get; set; }
        public string ApprovalBy { get; set; }
        public long? ToleranceCount { get; set; }
        public string ApprovalType { get; set; }
    }
    public class MarkersCompleteReport
    {
        public string Script { get; set; }
        public long? StdScore { get; set; }
        public long? MarkerScore { get; set; }
        public string WithinTolerance { get; set; }
        public string Result { get; set; }
        public List<QuestionDetails> queDetails { get; set; }
    }
    public class QuestionDetails
    {
        public string QuestionLabel { get; set; }
        public long? MaxMark { get; set; }
        public long? Tolerance { get; set; }
        public long? DefinitiveScore { get; set; }
        public long? MarkerScore { get; set; }
        public string Result { get; set; }
    }
    public class ApprovalDetails
    {
        
        public long UserRoleId { get; set; }
        public long ProjectUserRoleId { get; set; }
        public long QigId { get; set; }
        public string markingPersonal { get; set; }
        public string Remark { get; set; }

    }
    public class ApprovalDetailsModel
    {
        public ApprovalDetailsModel()
        {
            approvalDetailslist = new ApprovalDetails();
        }
        public ApprovalDetails approvalDetailslist { get; set; }  

        public long ProjectUserRoleId { set; get; }

        public string markingPersonal { get; set; }
        public string Remark { get; set; }
        public long QigId { get; set;}

    }
}
