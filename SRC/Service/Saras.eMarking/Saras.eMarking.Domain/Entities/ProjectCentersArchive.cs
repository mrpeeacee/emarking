using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectCentersArchive
{
    public long ArchiveId { get; set; }

    public long ProjectCenterId { get; set; }

    public long ProjectId { get; set; }

    public long CenterId { get; set; }

    public string CenterName { get; set; }

    public string CenterCode { get; set; }

    public long? TotalNoOfScripts { get; set; }

    public bool? IsSelectedForRecommendation { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public long? RecommendedBy { get; set; }

    public DateTime? RecommendationDate { get; set; }
}
