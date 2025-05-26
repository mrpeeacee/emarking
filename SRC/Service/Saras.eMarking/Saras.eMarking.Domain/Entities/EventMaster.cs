using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class EventMaster
{
    public long Id { get; set; }

    public string EventCode { get; set; }

    public string EventType { get; set; }

    public string Description { get; set; }

    public bool IsManualDriven { get; set; }
}
