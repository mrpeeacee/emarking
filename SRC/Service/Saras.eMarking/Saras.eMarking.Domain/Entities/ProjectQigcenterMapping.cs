using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectQigcenterMapping
{
    public long ProjectQigcenterId { get; set; }

    public long ProjectCenterId { get; set; }

    public long Qigid { get; set; }

    public long ProjectId { get; set; }

    public long? NoOfCandidates { get; set; }

    public bool IsDeleted { get; set; }

    public long RecommendedBy { get; set; }

    public DateTime RecommendedDate { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual ProjectUserRoleinfo ModifiedByNavigation { get; set; }

    public virtual ProjectInfo Project { get; set; }

    public virtual ProjectCenter ProjectCenter { get; set; }

    public virtual ProjectQig Qig { get; set; }

    public virtual ProjectUserRoleinfo RecommendedByNavigation { get; set; }
}
