using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels.Report
{
    /// <summary>
    /// Ems1 Report Response Model
    /// </summary>
    public class Ems1ReportModel
    {
        public int? ExamYear { get; set; }
        public string ExamLevelCode { get; set; }
        public string ExamSeriesCode { get; set; }
        public string SubjectCode { get; set; }
        public int? PaperNumber { get; set; }
        public string MOACode { get; set; }
        public string IndexNumber { get; set; }
        public byte? Attendance { get; set; }
        public long? QuestionID { get; set; }
        public string QuestionCode { get; set; }
        public decimal? ContentMark { get; set; }
        public decimal? LanguageOrganisationMark { get; set; }
        public decimal? TotalMark { get; set; }
        public string Results { get; set; }
    }

    /// <summary>
    /// Ems2 Report Response Model
    /// </summary>
    public class Ems2ReportModel
    {
        public int? ExamYear { get; set; }
        public string ExamLevelCode { get; set; }
        public string ExamSeriesCode { get; set; }
        public string SubjectCode { get; set; }
        public int? PaperNumber { get; set; }
        public string MOACode { get; set; }
        public string IndexNumber { get; set; }
        public byte? Attendance { get; set; }
        public string MarkerGroup { get; set; }
        public long? QuestionID { get; set; }
        public string QuestionCode { get; set; }
        public decimal? Mark { get; set; }
        public byte? SupervisorIndicator { get; set; }
        public string Results { get; set; }
    }

    /// <summary>
    /// Oms Response Model
    /// </summary>
    public class OmsReportModel
    {
        public string ExamYear { get; set; }
        public string ExamLevel { get; set; }
        public string ExamSeries { get; set; }
        public string SubjectCode { get; set; }
        public string PaperCode { get; set; }
        public string MOACode { get; set; }
        public string IndexNumber { get; set; }
        public string Attendance { get; set; }
        public string Mark { get; set; }
        public string Results { get; set; }
    }

    /// <summary>
    /// Download OutBound Log Model
    /// </summary>
    public class DownloadOutBoundLog
    {
        public long Count { get; set; }
        public string FileName { get; set; }
        public string Results { get; set; }
    }
    public class StudentResultReportModel
    {
        public long ProjectID { get; set; }
        public string LoginID { get; set; }
        public string Questioncode { get; set; }
        public long QIGID { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public long ProjectUserRoleID { get; set; }
    }

    public class StudentReport
    {
        public long ProjectID { get; set; }
        public long QIGID { get; set; }
        public string ScheduleUserID { get; set; }
        public long ProjectQuestionID { get; set; }
        public string QuestionVersion { get; set; }
        public string QuestionType { get; set; }
        public string QuestionCode { get; set; }
        public int RowNumber { get; set; }
        public long QuestionID { get; set; }
        public string LoginID { get; set; }
        public string QIGName { get; set; }
        public string Question_No { get; set; }
        public decimal? MaxMarks { get; set; }
        public decimal? Awarded_Marks { get; set; }
        public long? QuestionResponseMarkingID { get; set; }
        public string BandName { get; set; }
        public decimal? BandFrom { get; set; }
        public decimal? BandTo { get; set; }
        public long? ParentQuestionID { get; set; }
        public bool IsChildExists { get; set; }
        public List<ScoringComponentMarksModel> scoringComponentMarksModels { get; set; }
        public List<StudentReport> childfib { get; set; }
    }

    public class ScoringComponentMarksModel
    {

        public long? QuestionResponseMarkingID { get; set; }
        public long ProjectQuestionID { get; set; }
        public long QuestionID { get; set; }
        public string ComponentName { get; set; }
        public decimal MaxMarks { get; set; }
        public decimal AwardedMarks { get; set; }
        public string BandName { get; set; }
        public decimal? BandFrom { get; set; }
        public decimal? BandTo { get; set; }

    }

    public class QuestionCodeModel
    {

        public string QuestionCode { get; set; }
        public long QigId { get; set; }
        public long QuestionID { get; set; }

    }

    public class ReportsOutboundLogsModel
    {
        public Guid? CorrelationId { get; set; }
        public short? ReportType { get; set; }
        public DateTime? RequestDate { get; set; }
        public int? PageNo { get; set; }
        public int? PageSize { get; set; }
        public int? PageIndex { get; set; }
        public string FileName { get; set; }
        public bool? IsProcessed { get; set; }
        public DateTime? ProcessedOn { get; set; }
        public int? Status { get; set; }
        public string Remark { get; set; }
        public string RequestedBy { get; set; }
        public long ProjectUserRoleId { get; set; }
        public string ProjectUserRoleName { get; set;}
    }
    public class SyncOutboundResponseModel
    {
        public string Message { get; set; }
        public int Status { get; set; }
        public string Content { get; set; }
    }
    public class GetOralProjectClosureDetailsModel
    {
        public bool? IsReadyForSync { get; set; }
        public List<OralScheduleDetailsModel> scheduleDetails { get; set; }
        
    }
    public class OralScheduleDetailsModel
    {
        public string ScheduleCode { get; set; }
        public string ScheduleName { get; set; }
        public long? ScheduleID { get; set; }
    }
}
