using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class Privilege
{
    public long PrivilegeId { get; set; }

    public string PrivilegeCode { get; set; }

    public string PrivilegeName { get; set; }

    public string PrivilegeDescription { get; set; }

    public string PrivilegeUrl { get; set; }

    /// <summary>
    /// 1--&gt; Page 2--&gt; Module 3--&gt; Action 4--&gt; Quick Links
    /// </summary>
    public byte? PrivilegeType { get; set; }

    public string PrivilegeLevel { get; set; }

    public long? ParentPrivilegeId { get; set; }

    public bool IsLoggingRequired { get; set; }

    public bool IsKpprivilege { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? PrivilegeOrder { get; set; }

    public byte? PrivilegeGroup { get; set; }

    public bool IsProjectCloserPrivilege { get; set; }

    public virtual ICollection<Privilege> InverseParentPrivilege { get; set; } = new List<Privilege>();

    public virtual Privilege ParentPrivilege { get; set; }

    public virtual ICollection<RoleToPrivilege> RoleToPrivileges { get; set; } = new List<RoleToPrivilege>();
}
