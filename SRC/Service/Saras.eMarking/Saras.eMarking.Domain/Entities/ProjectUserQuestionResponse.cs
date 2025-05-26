using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectUserQuestionResponse
{
    public long ProjectUserQuestionResponseId { get; set; }

    public Guid ProjectUserQuestionResponseGuid { get; set; }

    public long ProjectId { get; set; }

    public long ScheduleUserId { get; set; }

    public long QuestionId { get; set; }

    public long? ProjectQuestionId { get; set; }

    public long? UserResponseId { get; set; }

    /// <summary>
    /// 1--&gt;Text, 2--&gt;Audio, 3--&gt;Video, 4--&gt;Documents
    /// </summary>
    public byte? ResponseType { get; set; }

    public string ResponsePath { get; set; }

    public string ResponseText { get; set; }

    public long? ScriptId { get; set; }

    public long? RecommendedBand { get; set; }

    public decimal? MaxScore { get; set; }

    public bool Isdeleted { get; set; }

    public string CandidateResponse { get; set; }

    public decimal? FinalizedMarks { get; set; }

    /// <summary>
    /// 1--&gt; Auto , 2--&gt; Moderated , 3 --&gt; Manual, 4-&gt; Post Live Marking Moderation
    /// </summary>
    public byte? MarkedType { get; set; }

    public long? MarkedBy { get; set; }

    public DateTime? MarkedDate { get; set; }

    public bool IsNullResponse { get; set; }

    public virtual ProjectUserRoleinfo MarkedByNavigation { get; set; }

    public virtual ICollection<MpstandardizationQueRespMarkingDetail> MpstandardizationQueRespMarkingDetails { get; set; } = new List<MpstandardizationQueRespMarkingDetail>();

    public virtual ProjectInfo Project { get; set; }

    public virtual ProjectQuestion ProjectQuestion { get; set; }

    public virtual ICollection<QuestionUserResponseMarkingDetail> QuestionUserResponseMarkingDetails { get; set; } = new List<QuestionUserResponseMarkingDetail>();

    public virtual ProjectMarkSchemeBandDetail RecommendedBandNavigation { get; set; }

    public virtual ProjectUserScript Script { get; set; }
}
