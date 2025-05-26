using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class PassPharse
{
    public long Id { get; set; }

    public string PassPharseCode { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<UserInfo> UserInfos { get; set; } = new List<UserInfo>();
}
