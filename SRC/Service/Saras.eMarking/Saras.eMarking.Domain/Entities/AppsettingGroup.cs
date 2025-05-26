using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class AppsettingGroup
{
    public int SettingGroupId { get; set; }

    public string SettingGroupCode { get; set; }

    public string SettingGroupName { get; set; }

    public string Description { get; set; }

    public long? OrganizationId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? CreatedBy { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }
}
