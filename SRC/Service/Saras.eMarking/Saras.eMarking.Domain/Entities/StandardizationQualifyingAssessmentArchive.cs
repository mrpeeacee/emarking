using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class StandardizationQualifyingAssessmentArchive
{
    public long ArchiveId { get; set; }

    public long QualifyingAssessmentId { get; set; }

    public long ProjectId { get; set; }

    public long Qigid { get; set; }

    public int? TotalNoOfScripts { get; set; }

    public int? NoOfScriptSelected { get; set; }

    public int ToleranceCount { get; set; }

    /// <summary>
    /// 1--&gt;Sequential, 2--&gt;Random
    /// </summary>
    public byte? ScriptPresentationType { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool IsTagged { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public long? RefQualifyingAssessmentId { get; set; }

    public byte ApprovalType { get; set; }
}
