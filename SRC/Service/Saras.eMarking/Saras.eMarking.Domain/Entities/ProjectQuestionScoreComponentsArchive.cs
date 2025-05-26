using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectQuestionScoreComponentsArchive
{
    public long ArchiveId { get; set; }

    public long ScoreComponentId { get; set; }

    public string ComponentCode { get; set; }

    public string ComponentName { get; set; }

    public string ComponentDescription { get; set; }

    public long ProjectQuestionId { get; set; }

    public decimal MaxMarks { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool IsAutoCreated { get; set; }
}
