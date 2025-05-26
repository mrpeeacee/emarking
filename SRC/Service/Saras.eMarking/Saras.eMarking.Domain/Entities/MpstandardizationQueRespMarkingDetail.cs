using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class MpstandardizationQueRespMarkingDetail
{
    public long Id { get; set; }

    public long ProjectId { get; set; }

    public long Qigid { get; set; }

    public long ScriptId { get; set; }

    public long ProjectUserRoleId { get; set; }

    public long ProjectQuestionResponceId { get; set; }

    public long StandardizationScriptMarkingRefId { get; set; }

    public decimal TotalMarks { get; set; }

    public decimal? DefenetiveMarks { get; set; }

    public decimal AwardedMarks { get; set; }

    public int? ToleranceLimit { get; set; }

    public bool IsOutOfTolerance { get; set; }

    public int WorkflowStatusId { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual ProjectInfo Project { get; set; }

    public virtual ProjectUserQuestionResponse ProjectQuestionResponce { get; set; }

    public virtual ProjectUserRoleinfo ProjectUserRole { get; set; }

    public virtual ProjectQig Qig { get; set; }

    public virtual ProjectUserScript Script { get; set; }

    public virtual MpstandardizationScriptMarkingDetail StandardizationScriptMarkingRef { get; set; }

    public virtual WorkflowStatus WorkflowStatus { get; set; }
}
