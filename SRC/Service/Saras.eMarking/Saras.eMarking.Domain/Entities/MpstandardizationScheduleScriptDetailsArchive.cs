using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class MpstandardizationScheduleScriptDetailsArchive
{
    public long ArchiveId { get; set; }

    public long StandardizationScheduleScriptDetailId { get; set; }

    public long StandardizationScheduleId { get; set; }

    public long ScriptId { get; set; }

    public int WorkflowStatusId { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime? StartDate { get; set; }

    public long? SubmittedBy { get; set; }

    public DateTime? SubmittedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? ScriptCategorizationPoolId { get; set; }

    public decimal? CategorizationVersion { get; set; }
}
