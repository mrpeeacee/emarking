using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class QuestionUserResponseMarkingDetail
{
    public long Id { get; set; }

    public long ScriptId { get; set; }

    public long ProjectQuestionResponseId { get; set; }

    public long? CandidateId { get; set; }

    public long? ScheduleUserId { get; set; }

    public string Annotation { get; set; }

    public string ImageBase64 { get; set; }

    public string Comments { get; set; }

    public long? BandId { get; set; }

    public decimal? Marks { get; set; }

    public bool? IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public long? MarkedBy { get; set; }

    public DateTime? Markeddate { get; set; }

    /// <summary>
    /// 1---&gt;Approved,  2---&gt;Ammended, 3---&gt;Rejected
    /// </summary>
    public byte? MarkingStatus { get; set; }

    public int? WorkflowstatusId { get; set; }

    public long? UserScriptMarkingRefId { get; set; }

    public bool LastVisited { get; set; }

    public string Remarks { get; set; }

    /// <summary>
    /// 1--&gt; Auto , 2--&gt; Moderated , 3 --&gt; Manual, 4-&gt; Post Live Marking Moderation
    /// </summary>
    public byte? MarkedType { get; set; }

    public virtual ProjectMarkSchemeBandDetail Band { get; set; }

    public virtual ProjectUserRoleinfo MarkedByNavigation { get; set; }

    public virtual ProjectUserQuestionResponse ProjectQuestionResponse { get; set; }

    public virtual ICollection<QuestionScoreComponentMarkingDetail> QuestionScoreComponentMarkingDetails { get; set; } = new List<QuestionScoreComponentMarkingDetail>();

    public virtual ICollection<QuestionUserResponseMarkingImage> QuestionUserResponseMarkingImages { get; set; } = new List<QuestionUserResponseMarkingImage>();

    public virtual ProjectUserScript Script { get; set; }

    public virtual UserScriptMarkingDetail UserScriptMarkingRef { get; set; }

    public virtual WorkflowStatus Workflowstatus { get; set; }
}
