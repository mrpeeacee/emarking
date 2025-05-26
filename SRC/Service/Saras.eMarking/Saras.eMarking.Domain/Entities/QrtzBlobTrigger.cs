using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class QrtzBlobTrigger
{
    public string SchedName { get; set; }

    public string TriggerName { get; set; }

    public string TriggerGroup { get; set; }

    public byte[] BlobData { get; set; }
}
