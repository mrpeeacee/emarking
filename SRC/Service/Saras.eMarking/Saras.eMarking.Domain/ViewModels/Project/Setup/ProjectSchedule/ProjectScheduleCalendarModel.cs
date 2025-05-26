using System;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Project.Setup.ProjectSchedule
{
    public class ProjectScheduleCalendarModel
    {
        [Required]
        public long ProjectScheduleID { get; set; }
        [Required]
        public long ProjectCalendarID { get; set; }
        public DateTime? CalendarDate { get; set; }
        [Range(1, 2, ErrorMessage = "Please enter a value bigger than or equal to {1}")]
        public byte DayType { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
    }
}
