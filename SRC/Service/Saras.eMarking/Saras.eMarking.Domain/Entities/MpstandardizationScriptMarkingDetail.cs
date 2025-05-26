using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class MpstandardizationScriptMarkingDetail
{
    public long StandardizationScriptMarkingId { get; set; }

    public long ProjectId { get; set; }

    public long Qigid { get; set; }

    public long ProjectUserRoleId { get; set; }

    public long ScriptId { get; set; }

    public long UserScriptMarkingRefId { get; set; }

    public decimal TotalMarks { get; set; }

    public decimal DefenitiveScriptMarks { get; set; }

    public decimal AwardedTotalMarks { get; set; }

    public int WorkflowStatusId { get; set; }

    public bool IsOutOfTolerance { get; set; }

    public int? TotalNoOfResponces { get; set; }

    public int? OutOfToleranceResponces { get; set; }

    public long? AssignedBy { get; set; }

    public DateTime? AssignedDate { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long StandardizationScheduleId { get; set; }

    public virtual ProjectUserRoleinfo AssignedByNavigation { get; set; }

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual ICollection<MpstandardizationQueRespMarkingDetail> MpstandardizationQueRespMarkingDetails { get; set; } = new List<MpstandardizationQueRespMarkingDetail>();

    public virtual ProjectInfo Project { get; set; }

    public virtual ProjectUserRoleinfo ProjectUserRole { get; set; }

    public virtual ProjectQig Qig { get; set; }

    public virtual ProjectUserScript Script { get; set; }

    public virtual MpstandardizationSchedule StandardizationSchedule { get; set; }

    public virtual UserScriptMarkingDetail UserScriptMarkingRef { get; set; }

    public virtual WorkflowStatus WorkflowStatus { get; set; }
}
