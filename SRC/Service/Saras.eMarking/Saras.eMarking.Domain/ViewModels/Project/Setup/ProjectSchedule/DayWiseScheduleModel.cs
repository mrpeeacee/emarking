using Saras.eMarking.Domain.Extensions;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using System;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Project.Setup.ProjectSchedule
{
    public class DayWiseScheduleModel : IAuditTrail
    {
        [Required]
        public long ProjectCalendarID { get; set; } 
        [Required]
        public DateTime CalendarDate { get; set; }
        public DateTime SelectedDate { get; set; }
        public byte DayType { get; set; }
        [Required]
        public TimeSpan StartTime { get; set; }
        [Required]
        public TimeSpan EndTime { get; set; }
        [XssTextValidation]
        [MaxLength(250)]
        public string Remarks { get; set; } 
        public DateTime ChoosenDate { get; set; }
        [Required]
        public DateTime StartDateTime { get; set; }
        [Required]
        public DateTime EndDateTime { get; set; }
    }
}
