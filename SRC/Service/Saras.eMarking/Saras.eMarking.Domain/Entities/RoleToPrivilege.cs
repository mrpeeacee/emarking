using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class RoleToPrivilege
{
    public long RoleToPrivilegeId { get; set; }

    public int RoleId { get; set; }

    public long? PrivilegeId { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual Privilege Privilege { get; set; }

    public virtual Roleinfo Role { get; set; }
}
