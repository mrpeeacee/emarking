using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class UserPwdDetailsArchive
{
    public long ArchiveId { get; set; }

    public long UserPwdDetailId { get; set; }

    public long UserId { get; set; }

    public string Password { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime? ActivationStartdate { get; set; }

    public DateTime? ActivationEnddate { get; set; }
}
