using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class TimeZone
{
    public int TimeZoneId { get; set; }

    public string TimeZoneCode { get; set; }

    public string TimeZoneName { get; set; }

    public int BaseUtcoffsetInMin { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<UserToTimeZoneMapping> UserToTimeZoneMappings { get; set; } = new List<UserToTimeZoneMapping>();
}
