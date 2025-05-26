using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectTeamUserInfo
{
    public long ProjectTeamUserId { get; set; }

    public long ProjectTeamId { get; set; }

    public long ProjectUserRoleId { get; set; }

    public long? ReportingTo { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? IsPromotedUser { get; set; }

    public bool? IsActive { get; set; }

    public bool Isdeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string Remarks { get; set; }

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual ProjectUserRoleinfo ModifiedByNavigation { get; set; }

    public virtual ProjectTeam ProjectTeam { get; set; }

    public virtual ProjectUserRoleinfo ProjectUserRole { get; set; }

    public virtual ProjectUserRoleinfo ReportingToNavigation { get; set; }
}
