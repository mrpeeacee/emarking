using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ScriptCategorizationPoolArchive
{
    public long ArchiveId { get; set; }

    public long ScriptCategorizationPoolId { get; set; }

    public long ProjectId { get; set; }

    public long ScriptId { get; set; }

    public long? Qigid { get; set; }

    /// <summary>
    /// 1--&gt;Standardization Script, 2--&gt;Adtnal.Standardization Script, 3--&gt;BenchMark Script
    /// </summary>
    public byte PoolType { get; set; }

    public long? UserScriptMarkingRefId { get; set; }

    public decimal? MaxMarks { get; set; }

    public decimal? FinalizedMarks { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? CreatedBy { get; set; }

    public bool IsDeleted { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public decimal? CategorizationVersion { get; set; }
}
