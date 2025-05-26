using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class ProjectSchedule
{
    public long ProjectScheduleId { get; set; }

    public long? ProjectId { get; set; }

    public string ScheduleName { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? ExpectedEndDate { get; set; }

    public DateTime? ActualEndDate { get; set; }

    public bool Isdeleted { get; set; }

    public DateTime CreatedDate { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public long? ModifiedBy { get; set; }

    public string WorkingDaysConfig { get; set; }

    public long? RefProjectScheduleId { get; set; }

    public bool IsActive { get; set; }

    public virtual ProjectUserRoleinfo CreatedByNavigation { get; set; }

    public virtual ICollection<ProjectSchedule> InverseRefProjectSchedule { get; set; } = new List<ProjectSchedule>();

    public virtual ProjectUserRoleinfo ModifiedByNavigation { get; set; }

    public virtual ProjectInfo Project { get; set; }

    public virtual ICollection<ProjectScheduleCalendar> ProjectScheduleCalendars { get; set; } = new List<ProjectScheduleCalendar>();

    public virtual ProjectSchedule RefProjectSchedule { get; set; }
}
