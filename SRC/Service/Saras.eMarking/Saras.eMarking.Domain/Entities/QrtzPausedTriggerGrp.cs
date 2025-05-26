using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class QrtzPausedTriggerGrp
{
    public string SchedName { get; set; }

    public string TriggerGroup { get; set; }
}
