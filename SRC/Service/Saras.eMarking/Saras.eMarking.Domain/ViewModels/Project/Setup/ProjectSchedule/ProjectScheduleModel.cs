using Newtonsoft.Json.Linq;
using Saras.eMarking.Domain.Extensions;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Project.Setup.ProjectSchedule
{
    public class ProjectScheduleModel : IAuditTrail
    {
        [XssTextValidation]
        public string ScheduleName { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime ExpectedEndDate { get; set; }
        [Required]
        public TimeSpan StartTime { get; set; }
        [Required]
        public TimeSpan EndTime { get; set; } 
        public List<ProjectScheduleCalendarModel> ScheduleTimeModels { get; set; }
        public Dictionary<string, string> WorkingDaysConfig { get; set; }
        public JObject WorkingDaysConfigJson { get; set; }
        public System.DateTime? CalendarDate { get; set; }
        public byte DayType { get; set; }
        [Range(1, 2, ErrorMessage = "Please enter a value bigger than or equal to {1}")]
        public int? Confirmdialogeventvalue { get; set; }
        public bool IsUpdate { get; set; }
    }
}
