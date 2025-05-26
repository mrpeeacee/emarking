using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Project.LiveMarking
{
    public class LiveMarkingModel
    {
        public int SubmitScriptDailyCount { get; set; }
        public int DownloadLimitCount { get; set; }
        public int LivescriptCount { get; set; }
        public int GraceperiodScript { get; set; }
        public int SubmittedScript { get; set; }
        public int ReallocatedScript { get; set; }
        public string QigName { get; set; }
        public string RoleCode { get; set; }
        public List<Livescripts> Livescripts { get; set; }


    }

    public class Livescripts
    {
        public long ScriptPhaseTrackingId{ get; set; }
        public long ScriptId { get; set; }
        public string ScriptName { get; set; }
        public long ProjectId { get; set; }
        public long MarkedBy { get; set; }
        public int WorkflowStatusID { get; set; }
        public byte phase { get; set; }
        public DateTime? GracePeriodEndDateTime { get; set; }
        public double Seconds { get; set; }
        public byte status { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public int? GracePeriodInMin { get; set; }
        public long? UserMarkRefID { get; set; }
        public string Remarks { get; set; }
        public decimal? TotalAwardedMarks { get; set; }
        public decimal? TotalMaxMarks { get; set; }
    }

    public class ProjectUserScripts
    {
        
        public long ScriptId { get; set; }
        public string ScriptName { get; set; }
        public long ProjectID { get; set; }
        public long QigID { get; set; }
        public int NumberOfQuestions { get; set; }
    }

    public class ClsLiveScript
    {
        [Required(ErrorMessage = "QigId is required")]
        public long QigID { get; set; }
        [Required(ErrorMessage = "Pool is required")]
        public int pool { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string RoleCode { get; set; }
    }

    public class Livepoolscript
    {
        public long QigID { get; set; }
        public long ProjectId { get; set; }
        public long ProjectUserRoleId { get; set; }
        public List<Scriptsoflivepool> scriptsids { get; set; }
    }

    public class Scriptsoflivepool
    {
        public long ScriptId { get; set; }
    }

}
