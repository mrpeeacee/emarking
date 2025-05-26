using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class UserToRoleMapping
{
    public long MappingId { get; set; }

    public long? UserId { get; set; }

    public int? RoleId { get; set; }

    public bool IsDeleted { get; set; }

    public long? OrganizationUserId { get; set; }

    public virtual UserToOrganizationMapping OrganizationUser { get; set; }

    public virtual Roleinfo Role { get; set; }

    public virtual UserInfo User { get; set; }
}
