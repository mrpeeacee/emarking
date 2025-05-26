using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class UserStatusTrackingArchive
{
    public long ArchiveId { get; set; }

    public long UserTrackingId { get; set; }

    public long? UserId { get; set; }

    public long? ProjectUserRoleId { get; set; }

    /// <summary>
    /// 1--&gt; Application Level , 2 --&gt; Project Level
    /// </summary>
    public byte? StatusLevel { get; set; }

    /// <summary>
    /// 1 --&gt; Disable, 2 --&gt; Map, 3 --&gt; Un-Map, 4 --&gt; Block,5 --&gt; Unblock,6 --&gt; Active,7 --&gt; InActive,8--&gt; Remove,9 --&gt; Promotion,10--&gt;Suspended,11--&gt; Resume, 12 -&gt; Tag, 13 -&gt; Untag, 14 -&gt; Retag, 15 -&gt; PasswordReset
    /// </summary>
    public int? Status { get; set; }

    public long? ActionByUserId { get; set; }

    public long? ActionByProjectUserRoleId { get; set; }

    public DateTime? ActionDate { get; set; }

    public string Remarks { get; set; }
}
