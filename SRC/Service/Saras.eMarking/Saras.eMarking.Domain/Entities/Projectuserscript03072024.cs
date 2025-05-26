using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class Projectuserscript03072024
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

    public byte? MarkedType { get; set; }

    public long? MarkedBy { get; set; }

    public DateTime? MarkedDate { get; set; }

    public long? UnRecommendedBy { get; set; }

    public DateTime? UnRecommendedDate { get; set; }

    public int? ScriptType { get; set; }

    public long? DownloadedBy { get; set; }
}
