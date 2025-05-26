using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class AppSettingsArchive
{
    public long ArchiveId { get; set; }

    public long AppSettingId { get; set; }

    public long? EntityId { get; set; }

    /// <summary>
    /// 1--&gt;Project, 2--&gt;QIG, 3--&gt;User, 4--&gt;Role, 5.Question
    /// </summary>
    public byte? EntityType { get; set; }

    public long AppSettingKeyId { get; set; }

    public string Value { get; set; }

    public string DefaultValue { get; set; }

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
}
