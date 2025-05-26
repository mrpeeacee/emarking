using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class UserScriptMarkingDetail
{
    public long Id { get; set; }

    public long ScriptId { get; set; }

    public long ProjectId { get; set; }

    public long? CandidateId { get; set; }

    public long? ScheduleUserId { get; set; }

    public byte? TotalNoOfQuestions { get; set; }

    public byte? MarkedQuestions { get; set; }

    /// <summary>
    /// 1--&gt;In Progress, 2--&gt;Completed
    /// </summary>
    public byte? ScriptMarkingStatus { get; set; }

    public int? WorkFlowStatusId { get; set; }

    public long? MarkedBy { get; set; }

    public DateTime? MarkedDate { get; set; }

    public bool IsDeleted { get; set; }

    public bool? IsActive { get; set; }

    public decimal? TotalMarks { get; set; }

    public bool SelectAsDefinitive { get; set; }

    public long? SelectedBy { get; set; }

    public DateTime? SelectedDate { get; set; }

    public byte ApprovalStatus { get; set; }

    public long? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public long? AnnotationTemplateId { get; set; }

    /// <summary>
    /// 1--&gt; Auto , 2--&gt; Moderated , 3 --&gt; Manual, 4-&gt; Post Live Marking Moderation
    /// </summary>
    public byte? MarkedType { get; set; }

    public bool IsAutoSave { get; set; }

    public virtual AnnotationTemplate AnnotationTemplate { get; set; }

    public virtual ProjectUserRoleinfo ApprovedByNavigation { get; set; }

    public virtual ProjectUserRoleinfo MarkedByNavigation { get; set; }

    public virtual ICollection<MarkingScriptTimeTracking> MarkingScriptTimeTrackings { get; set; } = new List<MarkingScriptTimeTracking>();

    public virtual ICollection<MpstandardizationScriptMarkingDetail> MpstandardizationScriptMarkingDetails { get; set; } = new List<MpstandardizationScriptMarkingDetail>();

    public virtual ProjectInfo Project { get; set; }

    public virtual ICollection<QuestionScoreComponentMarkingDetail> QuestionScoreComponentMarkingDetails { get; set; } = new List<QuestionScoreComponentMarkingDetail>();

    public virtual ICollection<QuestionUserResponseMarkingDetail> QuestionUserResponseMarkingDetails { get; set; } = new List<QuestionUserResponseMarkingDetail>();

    public virtual ICollection<QuestionUserResponseMarkingImage> QuestionUserResponseMarkingImages { get; set; } = new List<QuestionUserResponseMarkingImage>();

    public virtual ProjectUserScript Script { get; set; }

    public virtual ICollection<ScriptCategorizationPool> ScriptCategorizationPools { get; set; } = new List<ScriptCategorizationPool>();

    public virtual ICollection<ScriptMarkingPhaseStatusTracking> ScriptMarkingPhaseStatusTrackings { get; set; } = new List<ScriptMarkingPhaseStatusTracking>();

    public virtual ProjectUserRoleinfo SelectedByNavigation { get; set; }

    public virtual WorkflowStatus WorkFlowStatus { get; set; }
}
