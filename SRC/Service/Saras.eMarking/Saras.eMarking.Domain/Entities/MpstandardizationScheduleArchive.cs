using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class MpstandardizationScheduleArchive
{
    public long ArchiveId { get; set; }

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
}
