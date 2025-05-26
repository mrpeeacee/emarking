using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectWorkflowStatusTracking
{
    public long ProjectWorkflowTrackingId { get; set; }

    public long EntityId { get; set; }

    /// <summary>
    /// 1--&gt;Project, 2--&gt;QIG, 3--&gt;User, 4--&gt;Role, 5.Question
    /// </summary>
    public byte EntityType { get; set; }

    public int WorkflowStatusId { get; set; }

    /// <summary>
    /// 1--&gt;Started, 2--&gt;InProgress, 3--&gt;Completed, 4--&gt;OnHold, 5--&gt; Closure
    /// </summary>
    public byte ProcessStatus { get; set; }

    public string Remarks { get; set; }

    public bool IsDeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual WorkflowStatus WorkflowStatus { get; set; }
}
