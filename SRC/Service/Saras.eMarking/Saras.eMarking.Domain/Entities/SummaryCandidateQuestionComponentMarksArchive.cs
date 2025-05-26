using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class SummaryCandidateQuestionComponentMarksArchive
{
    public long ArchiveId { get; set; }

    public long Id { get; set; }

    public long ProjectId { get; set; }

    public long? Qigid { get; set; }

    public long? ProductId { get; set; }

    public long? ScheduleId { get; set; }

    public long? AssessmentId { get; set; }

    public decimal? AssessmentVersion { get; set; }

    public long? ScheduleUserId { get; set; }

    public int? Status { get; set; }

    public long? UserId { get; set; }

    public long? ProjectQuestionId { get; set; }

    public long? ParentQuestionId { get; set; }

    public long? QuestionId { get; set; }

    public decimal? QuestionVersion { get; set; }

    public int? QuestionType { get; set; }

    public decimal? MaxMarks { get; set; }

    public long? UserScriptMarkingRefId { get; set; }

    public long? ScriptId { get; set; }

    public int? MarkingType { get; set; }

    public long? ScoreComponentId { get; set; }

    public decimal? ComponentMaxMarks { get; set; }

    public decimal? ComponentAwardedMarks { get; set; }

    public long? MarkSchemeId { get; set; }

    public long? BandId { get; set; }

    public decimal? TotalAwardedMarks { get; set; }

    public int? ScriptType { get; set; }

    public bool? IsNullResponse { get; set; }

    public bool? IsDeleted { get; set; }

    public bool? IsSelectedFromOptionality { get; set; }
}
