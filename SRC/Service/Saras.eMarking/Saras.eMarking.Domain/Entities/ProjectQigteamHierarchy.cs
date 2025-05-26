using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectQigteamHierarchy
{
    public long ProjectQigteamHierarchyId { get; set; }

    public long ProjectId { get; set; }

    public long Qigid { get; set; }

    public long ProjectUserRoleId { get; set; }

    public long? ReportingTo { get; set; }

    public bool Isdeleted { get; set; }

    public bool IsActive { get; set; }

    public bool IsKp { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual ProjectUserRoleinfo ModifiedByNavigation { get; set; }

    public virtual ProjectInfo Project { get; set; }

    public virtual ProjectUserRoleinfo ProjectUserRole { get; set; }

    public virtual ProjectQig Qig { get; set; }

    public virtual ProjectUserRoleinfo ReportingToNavigation { get; set; }
}
