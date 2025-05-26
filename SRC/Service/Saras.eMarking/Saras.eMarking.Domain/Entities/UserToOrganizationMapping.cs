using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class UserToOrganizationMapping
{
    public long OrganizationUserId { get; set; }

    public long? UserId { get; set; }

    public long? OrganizationId { get; set; }

    public bool? IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual Organization Organization { get; set; }

    public virtual UserInfo User { get; set; }

    public virtual ICollection<UserToRoleMapping> UserToRoleMappings { get; set; } = new List<UserToRoleMapping>();
}
