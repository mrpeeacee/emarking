using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class AppSetting
{
    public long AppSettingId { get; set; }

    public long? EntityId { get; set; }

    /// <summary>
    /// 1--&gt;Project, 2--&gt;QIG, 3--&gt;User, 4--&gt;Role, 5.Question
    /// </summary>
    public byte? EntityType { get; set; }

    public long AppSettingKeyId { get; set; }

    public string Value { get; set; }

    public string DefaultValue { get; set; }

    /// <summary>
    /// 1--&gt;String, 2--&gt;Integer, 3--&gt;Float, 4--&gt;XML, 5--&gt;DateTime,6--&gt;Bit,7--&gt;Int,8--&gt;BigInt
    /// </summary>
    public byte? ValueType { get; set; }

    public long? ReferanceId { get; set; }

    public long? ProjectId { get; set; }

    public long? OrganizationId { get; set; }

    public bool Isdeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public long? AppsettingGroupId { get; set; }

    public virtual AppSettingKey AppSettingKey { get; set; }

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual ProjectUserRoleinfo ModifiedByNavigation { get; set; }

    public virtual ProjectInfo Project { get; set; }
}
