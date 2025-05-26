using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class MpstandardizationSchedule
{
    public long StandardizationScheduleId { get; set; }

    public long ProjectId { get; set; }

    public long Qigid { get; set; }

    public long ProjectUserRoleId { get; set; }

    public int WorkflowStatusId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public long? QualifyingAssessmentId { get; set; }

    public int? TotalNoOfScripts { get; set; }

    public long? AssignedBy { get; set; }

    public DateTime? AssignedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual ProjectUserRoleinfo AssignedByNavigation { get; set; }

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual ICollection<MpstandardizationScheduleScriptDetail> MpstandardizationScheduleScriptDetails { get; set; } = new List<MpstandardizationScheduleScriptDetail>();

    public virtual ICollection<MpstandardizationScriptMarkingDetail> MpstandardizationScriptMarkingDetails { get; set; } = new List<MpstandardizationScriptMarkingDetail>();

    public virtual ProjectInfo Project { get; set; }

    public virtual ProjectUserRoleinfo ProjectUserRole { get; set; }

    public virtual ProjectQig Qig { get; set; }

    public virtual StandardizationQualifyingAssessment QualifyingAssessment { get; set; }

    public virtual WorkflowStatus WorkflowStatus { get; set; }
}
