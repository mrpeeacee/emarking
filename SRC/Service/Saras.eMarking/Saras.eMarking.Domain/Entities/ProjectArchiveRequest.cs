using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectArchiveRequest
{
    public long RequestId { get; set; }

    public long ProjectId { get; set; }

    public long? RequestedBy { get; set; }

    public DateTime RequestedDate { get; set; }

    /// <summary>
    /// 0 --&gt; In Progress,1 --&gt; Archive Completed  waiting for user confirmation,2 --&gt; User Confirmed,3 --&gt; Archive Completed and Removed the live data,4 --&gt; Error Found
    /// </summary>
    public byte RequestStatus { get; set; }

    public DateTime? RequestStatusDate { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public long? Modifiedby { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public string Remarks { get; set; }

    public virtual ICollection<ProjectArchiveRequestDetail> ProjectArchiveRequestDetails { get; set; } = new List<ProjectArchiveRequestDetail>();
}
