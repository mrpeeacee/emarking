using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class MpstandardizationQueRespMarkingDetailsArchive
{
    public long ArchiveId { get; set; }

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
}
