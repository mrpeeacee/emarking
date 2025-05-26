using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectWorkflow
{
    public long ProjectWorkflowId { get; set; }

    public long ProjectId { get; set; }

    public long ApplicationWorkflowId { get; set; }

    public bool IsEnabled { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? DisabledDate { get; set; }

    public long? CreatedBy { get; set; }

    public long? ModifiedBy { get; set; }
}
