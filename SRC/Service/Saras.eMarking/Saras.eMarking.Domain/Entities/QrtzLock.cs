using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class QrtzLock
{
    public string SchedName { get; set; }

    public string LockName { get; set; }
}
