using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.Entities;

public partial class SummaryProjectUserResultDetailsArchive
{
    public long ArchiveId { get; set; }

    public long Id { get; set; }

    public long ProjectId { get; set; }

    public long? Qigid { get; set; }

    public long ProductId { get; set; }

    public int? SchoolId { get; set; }

    public long? AssessmentId { get; set; }

    public decimal? AssessmentVersion { get; set; }

    public long ScheduleUserId { get; set; }

    public long? ProjectQuestionId { get; set; }

    public long? QuestionId { get; set; }

    public decimal? QuestionVersion { get; set; }

    public int? QuestionType { get; set; }

    public decimal? MaxMarks { get; set; }

    public decimal? TotalMarks { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? IsSynced { get; set; }

    /// <summary>
    /// 1 --&gt; EMS1, 2 --&gt; EMS2,
    /// </summary>
    public short? ReportType { get; set; }

    public decimal? LanguageMarks { get; set; }

    public decimal? ContentMarks { get; set; }

    public bool IsDeleted { get; set; }

    public bool Attendance { get; set; }
}
