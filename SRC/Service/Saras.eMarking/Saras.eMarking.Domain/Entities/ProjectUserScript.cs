using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectUserScript
{
    public long ScriptId { get; set; }

    public Guid ScriptGuid { get; set; }

    public string ScriptName { get; set; }

    public long ProjectId { get; set; }

    public long Qigid { get; set; }

    public int? TotalNoOfQuestions { get; set; }

    public int? TotalNoOfResponses { get; set; }

    public long? UserId { get; set; }

    public long ScheduleUserId { get; set; }

    public bool Isdeleted { get; set; }

    public long? ProjectCenterId { get; set; }

    public bool? IsRecommended { get; set; }

    public long? RecommendedBy { get; set; }

    public DateTime? RecommendedDate { get; set; }

    public int? WorkflowStatusId { get; set; }

    public decimal? TotalMaxMarks { get; set; }

    public decimal? TotalMarksAwarded { get; set; }

    /// <summary>
    /// 1--&gt; Auto , 2--&gt; Moderated , 3 --&gt; Manual, 4-&gt; Post Live Marking Moderation
    /// </summary>
    public byte? MarkedType { get; set; }

    public long? MarkedBy { get; set; }

    public DateTime? MarkedDate { get; set; }

    public long? UnRecommendedBy { get; set; }

    public DateTime? UnRecommendedDate { get; set; }

    /// <summary>
    /// 1-&gt; Null Response, 2-&gt; Partial Response,3-&gt; Complete Response
    /// </summary>
    public int? ScriptType { get; set; }

    public long? DownloadedBy { get; set; }

    public virtual ProjectUserRoleinfo MarkedByNavigation { get; set; }

    public virtual ICollection<MpstandardizationQueRespMarkingDetail> MpstandardizationQueRespMarkingDetails { get; set; } = new List<MpstandardizationQueRespMarkingDetail>();

    public virtual ICollection<MpstandardizationScheduleScriptDetail> MpstandardizationScheduleScriptDetails { get; set; } = new List<MpstandardizationScheduleScriptDetail>();

    public virtual ICollection<MpstandardizationScriptMarkingDetail> MpstandardizationScriptMarkingDetails { get; set; } = new List<MpstandardizationScriptMarkingDetail>();

    public virtual ProjectInfo Project { get; set; }

    public virtual ProjectCenter ProjectCenter { get; set; }

    public virtual ICollection<ProjectUserQuestionResponse> ProjectUserQuestionResponses { get; set; } = new List<ProjectUserQuestionResponse>();

    public virtual ProjectQig Qig { get; set; }

    public virtual ICollection<QuestionUserResponseMarkingDetail> QuestionUserResponseMarkingDetails { get; set; } = new List<QuestionUserResponseMarkingDetail>();

    public virtual ProjectUserRoleinfo RecommendedByNavigation { get; set; }

    public virtual ICollection<ScriptCategorizationPool> ScriptCategorizationPools { get; set; } = new List<ScriptCategorizationPool>();

    public virtual ICollection<ScriptMarkingPhaseStatusTracking> ScriptMarkingPhaseStatusTrackings { get; set; } = new List<ScriptMarkingPhaseStatusTracking>();

    public virtual ProjectUserRoleinfo UnRecommendedByNavigation { get; set; }

    public virtual ICollection<UserScriptMarkingDetail> UserScriptMarkingDetails { get; set; } = new List<UserScriptMarkingDetail>();

    public virtual WorkflowStatus WorkflowStatus { get; set; }
}
