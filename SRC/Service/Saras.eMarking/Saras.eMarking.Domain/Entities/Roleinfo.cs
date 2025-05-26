using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class Roleinfo
{
    public int RoleId { get; set; }

    public string RoleCode { get; set; }

    public string RoleName { get; set; }

    public byte? RoleLevelId { get; set; }

    public bool Isdeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public short? ApplicationLevel { get; set; }

    /// <summary>
    /// 1-&gt; Application, 2-&gt; Project
    /// </summary>
    public short? RoleGroup { get; set; }

    public short? ParentRoleId { get; set; }

    public bool? IsChildExist { get; set; }

    public virtual ICollection<ProjectUserRoleinfo> ProjectUserRoleinfos { get; set; } = new List<ProjectUserRoleinfo>();

    public virtual ICollection<RoleToPrivilege> RoleToPrivileges { get; set; } = new List<RoleToPrivilege>();

    public virtual ICollection<UserToRoleMapping> UserToRoleMappings { get; set; } = new List<UserToRoleMapping>();
}
