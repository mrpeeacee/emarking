using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class UserScriptMarkingDetails03072024
{
    public long Id { get; set; }

    public long ScriptId { get; set; }

    public long ProjectId { get; set; }

    public long? CandidateId { get; set; }

    public long? ScheduleUserId { get; set; }

    public byte? TotalNoOfQuestions { get; set; }

    public byte? MarkedQuestions { get; set; }

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

    public byte? MarkedType { get; set; }

    public bool IsAutoSave { get; set; }
}
