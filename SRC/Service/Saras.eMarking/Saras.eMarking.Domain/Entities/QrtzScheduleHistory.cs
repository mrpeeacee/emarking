using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class QrtzScheduleHistory
{
    public long Id { get; set; }

    public string SchedName { get; set; }

    public string JobName { get; set; }

    public string JobGroup { get; set; }

    public string JobClass { get; set; }

    public string TriggerName { get; set; }

    public string TriggerGroup { get; set; }

    public long NextFireTime { get; set; }

    public long PrevFireTime { get; set; }

    public long StartTime { get; set; }

    public long? RepeatInterval { get; set; }

    public int? TimesTriggered { get; set; }

    public long? ProjectId { get; set; }

    public long? QigId { get; set; }

    public int? RcType { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }
}
