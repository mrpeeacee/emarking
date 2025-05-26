using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class UserToTimeZoneMapping
{
    public long TimeZoneMappingId { get; set; }

    public long UserId { get; set; }

    public int TimeZoneId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual TimeZone TimeZone { get; set; }

    public virtual UserInfo User { get; set; }
}
