using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectQigscriptsImportEventsArchive
{
    public long ArchiveId { get; set; }

    public long Id { get; set; }

    public long? ProjectId { get; set; }

    public long? Qigid { get; set; }

    public long? SetUpFinalizedBy { get; set; }

    public DateTime? SetUpFinalizedDate { get; set; }

    public bool IsProcessed { get; set; }

    /// <summary>
    /// 0 -- &gt; Pending 1 --&gt;In-Progree  2 --&gt; Completed 3 --&gt; Failed
    /// </summary>
    public byte? JobStatus { get; set; }

    public bool IsNextRunRequired { get; set; }

    public string ErrorLog { get; set; }

    public DateTime? ProcessedDate { get; set; }

    public bool IsNotified { get; set; }
}
