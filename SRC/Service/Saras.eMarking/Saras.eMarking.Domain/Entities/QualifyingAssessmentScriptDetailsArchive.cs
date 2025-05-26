using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class QualifyingAssessmentScriptDetailsArchive
{
    public long ArchiveId { get; set; }

    public long QassessmentScriptId { get; set; }

    public long QualifyingAssessmentId { get; set; }

    public long ScriptCategorizationPoolId { get; set; }

    public bool IsSelected { get; set; }

    public int? OrderIndex { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
