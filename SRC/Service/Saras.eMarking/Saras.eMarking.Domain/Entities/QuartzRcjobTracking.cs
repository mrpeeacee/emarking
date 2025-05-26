using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class QuartzRcjobTracking
{
    public long Id { get; set; }

    public string SchedName { get; set; }

    public string JobName { get; set; }

    public string JobGuid { get; set; }

    public string JobGroup { get; set; }

    public long? ProjectId { get; set; }

    public long? Qigid { get; set; }

    public int? Rctype { get; set; }

    public int? Duration { get; set; }

    public decimal? SamplingRate { get; set; }

    public DateTime? JobRunDateTime { get; set; }

    public string Remarks { get; set; }

    /// <summary>
    /// 1-&gt; Success, 2-&gt; Failure
    /// </summary>
    public byte? JobStatus { get; set; }

    public string ProcessedScripts { get; set; }
}
