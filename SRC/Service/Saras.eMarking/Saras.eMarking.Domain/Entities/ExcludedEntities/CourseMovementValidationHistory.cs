using System;

namespace Saras.eMarking.Domain.Entities
{
    public partial class CourseMovementValidationHistory
    {
        public long HistoryId { get; set; }
        public long Id { get; set; }
        public DateTime HistoryCreatedDate { get; set; }
        public long ScheduleId { get; set; }
        public DateTime? ExamCloseDate { get; set; }
        public DateTime? LoadedDataForEmarking { get; set; }
        public bool IsExamClosed { get; set; }
        public DateTime? JobRunDate { get; set; }
        public bool IsJobRunRequired { get; set; }
        public string JobStatus { get; set; }
        public DateTime? ValidationEndDate { get; set; }
        public int? NoOfValidationDays { get; set; }
        public bool IsReadyForEmarkingProcess { get; set; }
        public bool IsProjectCreated { get; set; }
        public long? ProjectId { get; set; }
        public DateTime? ProjectCreatedDate { get; set; }
        public string ProjectCreationStatus { get; set; }
        public DateTime? MpimportedDate { get; set; }
        public bool IsMpimported { get; set; }
        public bool IsScriptsImported { get; set; }
        public string MpimportedStatus { get; set; }
        public string ScriptsImportedStatus { get; set; }
        public DateTime? ScriptsImportedDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
