using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class AppSettingKey
{
    public long AppsettingKeyId { get; set; }

    public string AppsettingKey1 { get; set; }

    public string AppsettingKeyName { get; set; }

    public string Description { get; set; }

    public int? SettingGroupId { get; set; }

    public long? ParentAppsettingKeyId { get; set; }

    public long? OrganizationId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? CreatedBy { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    /// <summary>
    /// 1-Project, 2- QIG
    /// </summary>
    public byte? AppSettingType { get; set; }

    public virtual ICollection<AppSetting> AppSettings { get; set; } = new List<AppSetting>();
}
