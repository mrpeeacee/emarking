using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectCreationJobHistory
{
    public long JobId { get; set; }

    public string JobCode { get; set; }

    public string JobName { get; set; }

    public DateTime JobrunDateTime { get; set; }

    public long? ProjectId { get; set; }

    public long? ScheduleId { get; set; }

    public bool IsProcessed { get; set; }

    public string Remarks { get; set; }
}
