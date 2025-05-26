using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectTeam
{
    public long ProjectTeamId { get; set; }

    public long ProjectId { get; set; }

    public string TeamCode { get; set; }

    public string TeamName { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? IsActive { get; set; }

    public int? CenterId { get; set; }

    public bool Isdeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string Remarks { get; set; }

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual ProjectUserRoleinfo ModifiedByNavigation { get; set; }

    public virtual ProjectInfo Project { get; set; }

    public virtual ICollection<ProjectTeamQig> ProjectTeamQigs { get; set; } = new List<ProjectTeamQig>();

    public virtual ICollection<ProjectTeamUserInfo> ProjectTeamUserInfos { get; set; } = new List<ProjectTeamUserInfo>();
}
