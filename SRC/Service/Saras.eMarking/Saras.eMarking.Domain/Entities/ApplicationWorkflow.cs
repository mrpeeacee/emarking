using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ApplicationWorkflow
{
    public long WorkflowId { get; set; }

    public string WorkflowCode { get; set; }

    public string WorkflowName { get; set; }

    public long? ParentworkflowId { get; set; }

    public bool Isdeleted { get; set; }
}
