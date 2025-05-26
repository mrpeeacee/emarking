using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class MarkingScriptTimeTrackingArchive
{
    public long ArchiveId { get; set; }

    public long Id { get; set; }

    public long ProjectId { get; set; }

    public long Qigid { get; set; }

    public long ProjectQuestionId { get; set; }

    public long UserScriptMarkingRefId { get; set; }

    public int WorkFlowStatusId { get; set; }

    public long ProjectUserRoleId { get; set; }

    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// 1 --&gt; View , 2 --&gt; Edit
    /// </summary>
    public short? Mode { get; set; }

    public short? Action { get; set; }

    public TimeOnly? TimeTaken { get; set; }
}
