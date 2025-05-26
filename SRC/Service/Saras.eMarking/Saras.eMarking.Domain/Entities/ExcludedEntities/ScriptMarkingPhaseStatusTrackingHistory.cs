using System;

namespace Saras.eMarking.Domain.Entities
{
    public partial class ScriptMarkingPhaseStatusTrackingHistory
    {
        public long Id { get; set; }
        public long? PhaseStatusTrackingId { get; set; }
        public long ScriptId { get; set; }
        /// <summary>
        /// 1 --&gt; Live Marking, 2 --&gt; RC - 1, 3 --&gt; RC - 2, 4 --&gt; Ad-hoc, 5 --&gt; Escalate
        /// </summary>
        public byte Phase { get; set; }
        public long ProjectId { get; set; }
        public long Qigid { get; set; }

        /// <summary>
        /// 1 --&gt; Downloaded, 2 --&gt; In Progress, 3 --&gt; Submitted, 4 --&gt; In RC Pool, 5 --&gt; Approved, 6 --&gt; Re-Marking, 7 --&gt; RE-Submitted, 8 --&gt; Escalate, 9 --&gt; Invalidate and Re-Mark, 10 --&gt; Invalidate and Live Pool, 11 --&gt; Return to Live Pool
        /// </summary>
        public byte? Status { get; set; }
        public DateTime? ActionDate { get; set; }
        public long? ActionBy { get; set; }
        public int? GracePeriodInMin { get; set; }
        public bool IsRcjobRun { get; set; }
        public DateTime? GracePeriodEndDateTime { get; set; }
        public DateTime? RcjobRunDateTime { get; set; }
        public long? AssignedTo { get; set; }
        public bool IsActive { get; set; }
        public long? UserScriptMarkingRefId { get; set; }
        public string Comments { get; set; }
        public decimal? TotalAwardedMarks { get; set; }
        public bool IsDeleted { get; set; }
        public long? ScriptInitiatedBy { get; set; }
        public DateTime? AssignedToDateTime { get; set; }
        public long? PreviousActionBy { get; set; }
        /// <summary>
        /// 1--&gt;JOB , 2--&gt;APPLICATION
        /// </summary>
        public int? RecordType { get; set; }
        public bool IsScriptFinalised { get; set; }
        public DateTime HistoryCreatedDate { get; set; }

    }
}
