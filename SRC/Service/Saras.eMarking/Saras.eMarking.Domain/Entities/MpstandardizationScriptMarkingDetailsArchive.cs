using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class MpstandardizationScriptMarkingDetailsArchive
{
    public long ArchiveId { get; set; }

    public long StandardizationScriptMarkingId { get; set; }

    public long ProjectId { get; set; }

    public long Qigid { get; set; }

    public long ProjectUserRoleId { get; set; }

    public long ScriptId { get; set; }

    public long UserScriptMarkingRefId { get; set; }

    public decimal TotalMarks { get; set; }

    public decimal DefenitiveScriptMarks { get; set; }

    public decimal AwardedTotalMarks { get; set; }

    public int WorkflowStatusId { get; set; }

    public bool IsOutOfTolerance { get; set; }

    public int? TotalNoOfResponces { get; set; }

    public int? OutOfToleranceResponces { get; set; }

    public long? AssignedBy { get; set; }

    public DateTime? AssignedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long StandardizationScheduleId { get; set; }
}
