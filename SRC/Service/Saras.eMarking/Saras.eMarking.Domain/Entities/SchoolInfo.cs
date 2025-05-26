using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class SchoolInfo
{
    public int SchoolId { get; set; }

    public string SchoolCode { get; set; }

    public string SchoolName { get; set; }

    public string Address { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ProjectId { get; set; }

    public int? ParentId { get; set; }

    public virtual ICollection<ProjectCenterSchoolMapping> ProjectCenterSchoolMappings { get; set; } = new List<ProjectCenterSchoolMapping>();

    public virtual ICollection<ProjectUserRoleinfo> ProjectUserRoleinfos { get; set; } = new List<ProjectUserRoleinfo>();

    public virtual ICollection<ProjectUserSchoolMapping> ProjectUserSchoolMappings { get; set; } = new List<ProjectUserSchoolMapping>();

    public virtual ICollection<UserInfo> UserInfos { get; set; } = new List<UserInfo>();
}
