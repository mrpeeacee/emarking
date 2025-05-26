using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class QuestionScoreComponentMarkingDetailsArchive
{
    public long ArchiveId { get; set; }

    public long Id { get; set; }

    public long UserScriptMarkingRefId { get; set; }

    public long QuestionUserResponseMarkingRefId { get; set; }

    public long ScoreComponentId { get; set; }

    public decimal? MaxMarks { get; set; }

    public decimal? AwardedMarks { get; set; }

    public long? BandId { get; set; }

    /// <summary>
    /// 1---&gt;Approved,  2---&gt;Ammended, 3---&gt;Rejected
    /// </summary>
    public byte? MarkingStatus { get; set; }

    public int WorkflowStatusId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public long MarkedBy { get; set; }

    public DateTime MarkedDate { get; set; }
}
