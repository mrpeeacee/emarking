using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectCandidateWithdraw
{
    public long Id { get; set; }

    public long ProjectId { get; set; }

    public string IndexNumber { get; set; }

    public long ScheduleUserId { get; set; }

    public bool IsDeleted { get; set; }

    public long? WithDrawBy { get; set; }

    public DateTime? WithDrawDate { get; set; }

    public long? UnWithDrawBy { get; set; }

    public DateTime? UnWithDrawDate { get; set; }

    public virtual ProjectInfo Project { get; set; }

    public virtual ProjectUserRoleinfo UnWithDrawByNavigation { get; set; }

    public virtual ProjectUserRoleinfo WithDrawByNavigation { get; set; }
}
