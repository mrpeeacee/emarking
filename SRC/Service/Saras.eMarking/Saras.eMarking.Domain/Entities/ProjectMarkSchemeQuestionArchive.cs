using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectMarkSchemeQuestionArchive
{
    public long ArchiveId { get; set; }

    public long Id { get; set; }

    public long ProjectId { get; set; }

    public long ProjectQuestionId { get; set; }

    public long ProjectMarkSchemeId { get; set; }

    public bool Isdeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public long? ScoreComponentId { get; set; }
}
