using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectScheduleCalendarArchive
{
    public long ArchiveId { get; set; }

    public long ProjectCalendarId { get; set; }

    public long ProjectScheduleId { get; set; }

    public DateOnly? CalendarDate { get; set; }

    /// <summary>
    /// 1-Working;2-Holiday;3-Weekend;4-NotWorking(Others)
    /// </summary>
    public byte DayType { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public bool Isdeleted { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public DateTime? Modifieddate { get; set; }

    public string Remarks { get; set; }
}
