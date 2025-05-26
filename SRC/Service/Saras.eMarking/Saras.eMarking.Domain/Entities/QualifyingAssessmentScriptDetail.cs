using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class QualifyingAssessmentScriptDetail
{
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

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual ProjectUserRoleinfo ModifiedByNavigation { get; set; }

    public virtual StandardizationQualifyingAssessment QualifyingAssessment { get; set; }

    public virtual ScriptCategorizationPool ScriptCategorizationPool { get; set; }
}
