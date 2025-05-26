using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ScriptMarkingPhaseStatusTrackingArchive
{
    public long ArchiveId { get; set; }

    public long PhaseStatusTrackingId { get; set; }

    public long ProjectId { get; set; }

    public long Qigid { get; set; }

    public long ScriptId { get; set; }

    /// <summary>
    /// 1 --&gt; Live Marking, 2 --&gt; RC - 1, 3 --&gt; RC - 2, 4 --&gt; Ad-hoc, 5 --&gt; Escalate
    /// </summary>
    public byte Phase { get; set; }

    public byte? Status { get; set; }

    public long? ActionBy { get; set; }

    public DateTime? ActionDate { get; set; }

    public int? GracePeriodInMin { get; set; }

    public DateTime? GracePeriodEndDateTime { get; set; }

    public bool IsRcjobRun { get; set; }

    public DateTime? RcjobRunDateTime { get; set; }

    public bool IsActive { get; set; }

    public long? AssignedTo { get; set; }

    public long? UserScriptMarkingRefId { get; set; }

    public decimal? TotalAwardedMarks { get; set; }

    public string Comments { get; set; }

    public bool IsDeleted { get; set; }

    public long? PreviousActionBy { get; set; }

    public long? ScriptInitiatedBy { get; set; }

    public bool IsScriptFinalised { get; set; }

    public DateTime? AssignedToDateTime { get; set; }
}
