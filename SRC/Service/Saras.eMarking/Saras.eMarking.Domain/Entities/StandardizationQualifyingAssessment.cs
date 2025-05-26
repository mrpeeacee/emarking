using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class StandardizationQualifyingAssessment
{
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

    /// <summary>
    /// 1--&gt;Manual, 2--&gt;Automatic
    /// </summary>
    public byte ApprovalType { get; set; }

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual ICollection<StandardizationQualifyingAssessment> InverseRefQualifyingAssessment { get; set; } = new List<StandardizationQualifyingAssessment>();

    public virtual ProjectUserRoleinfo ModifiedByNavigation { get; set; }

    public virtual ICollection<MpstandardizationSchedule> MpstandardizationSchedules { get; set; } = new List<MpstandardizationSchedule>();

    public virtual ProjectInfo Project { get; set; }

    public virtual ProjectQig Qig { get; set; }

    public virtual ICollection<QualifyingAssessmentScriptDetail> QualifyingAssessmentScriptDetails { get; set; } = new List<QualifyingAssessmentScriptDetail>();

    public virtual StandardizationQualifyingAssessment RefQualifyingAssessment { get; set; }
}
