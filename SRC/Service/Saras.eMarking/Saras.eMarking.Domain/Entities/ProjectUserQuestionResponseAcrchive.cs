using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectUserQuestionResponseAcrchive
{
    public long ArchiveId { get; set; }

    public long ProjectUserQuestionResponseId { get; set; }

    public Guid ProjectUserQuestionResponseGuid { get; set; }

    public long ProjectId { get; set; }

    public long ScheduleUserId { get; set; }

    public long QuestionId { get; set; }

    public long? ProjectQuestionId { get; set; }

    public long? UserResponseId { get; set; }

    public byte? ResponseType { get; set; }

    public string ResponsePath { get; set; }

    public string ResponseText { get; set; }

    public long? ScriptId { get; set; }

    public long? RecommendedBand { get; set; }

    public decimal? MaxScore { get; set; }

    public bool Isdeleted { get; set; }

    public string CandidateResponse { get; set; }

    public decimal? FinalizedMarks { get; set; }

    public byte? MarkedType { get; set; }

    public long? MarkedBy { get; set; }

    public DateTime? MarkedDate { get; set; }

    public bool IsNullResponse { get; set; }
}
