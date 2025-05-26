using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectQigreset
{
    public long Id { get; set; }

    public long? ProjectId { get; set; }

    public long? Qigid { get; set; }

    public long? ResetBy { get; set; }

    public DateTime? ResetDate { get; set; }

    public long? AuthenticateBy { get; set; }

    public DateTime? AuthenticateDate { get; set; }

    public virtual ProjectUserRoleinfo AuthenticateByNavigation { get; set; }

    public virtual ProjectInfo Project { get; set; }

    public virtual ProjectQig Qig { get; set; }

    public virtual ProjectUserRoleinfo ResetByNavigation { get; set; }
}
