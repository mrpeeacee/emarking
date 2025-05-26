using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectUserSchoolMapping
{
    public long Id { get; set; }

    public long? ProjectUserRoleId { get; set; }

    public int? ExemptionSchoolId { get; set; }

    public bool IsSendingSchool { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual SchoolInfo ExemptionSchool { get; set; }

    public virtual ProjectUserRoleinfo ModifiedByNavigation { get; set; }

    public virtual ProjectUserRoleinfo ProjectUserRole { get; set; }
}
