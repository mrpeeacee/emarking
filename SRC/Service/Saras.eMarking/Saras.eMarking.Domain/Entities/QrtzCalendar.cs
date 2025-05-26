using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class QrtzCalendar
{
    public string SchedName { get; set; }

    public string CalendarName { get; set; }

    public byte[] Calendar { get; set; }
}
