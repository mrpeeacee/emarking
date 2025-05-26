using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectTeamQig
{
    public long TeamQigid { get; set; }

    public long ProjectId { get; set; }

    public long TeamId { get; set; }

    public long? Qigid { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime AssignedDate { get; set; }

    public long? AssignedBy { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ProjectUserRoleinfo AssignedByNavigation { get; set; }

    public virtual ProjectUserRoleinfo ModifiedByNavigation { get; set; }

    public virtual ProjectInfo Project { get; set; }

    public virtual ProjectQig Qig { get; set; }

    public virtual ProjectTeam Team { get; set; }
}
