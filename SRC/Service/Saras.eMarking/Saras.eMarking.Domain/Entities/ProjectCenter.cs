using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectCenter
{
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

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual ProjectUserRoleinfo ModifiedByNavigation { get; set; }

    public virtual ProjectInfo Project { get; set; }

    public virtual ICollection<ProjectCenterSchoolMapping> ProjectCenterSchoolMappings { get; set; } = new List<ProjectCenterSchoolMapping>();

    public virtual ICollection<ProjectQigcenterMapping> ProjectQigcenterMappings { get; set; } = new List<ProjectQigcenterMapping>();

    public virtual ICollection<ProjectUserScript> ProjectUserScripts { get; set; } = new List<ProjectUserScript>();
}
