using Microsoft.AspNetCore.Http;
using Saras.eMarking.Domain.ViewModels.Project.MarkScheme;
using Saras.eMarking.Domain.ViewModels.Project.Setup.QigManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Saras.eMarking.Domain.ViewModels.AdminTools
{
    /// <summary>
    /// Admin Tools
    /// </summary>
    public class AdminToolsModel
    {
        public List<RC1ReportModel> rc1report { get; set; }
        public List<RC2ReportModel> rc2report { get; set; }
        public List<AdHocReportModel> adhocreport { get; set; }
        public string status { get; set; }
    }

    /// <summary>
    /// Live Marking Progress Model
    /// </summary>
    public class LiveMarkingProgressModel
    {
        public string MarkingProject { get; set; }
        public string QIGName { get; set; }
        public string DownloadedDateTime { get; set; }
        public int TotalManualMarkingScript { get; set; }
        public int DownloadedScripts { get; set; }
        public int ActionNeeded { get; set; }
        public int TotalPending { get; set; }
        public int TotalMarked { get; set; }
        public int CompletionRate { get; set; }
        public string status { get; set; }
    }

    /// <summary>
    /// RC1 Report Model
    /// </summary>
    public class RC1ReportModel
    {
        public string MarkingProject { get; set; }
        public string QIGName { get; set; }
        public int TotalScript { get; set; }
        public int TotalInProgressScript { get; set; }
        public int CheckOutScripts { get; set; }
        public int TotalCompleted { get; set; }
        public decimal CompletionRateInPercentage { get; set; }
    }

    /// <summary>
    /// RC2 Report Model
    /// </summary>
    public class RC2ReportModel
    {
        public string MarkingProject { get; set; }
        public string QIGName { get; set; }
        public int TotalScript { get; set; }
        public int TotalInProgressScript { get; set; }
        public int CheckOutScripts { get; set; }
        public int TotalCompleted { get; set; }
        public int CompletionRateInPercentage { get; set; }
    }

    /// <summary>
    /// AdHoc Report Model
    /// </summary>
    public class AdHocReportModel
    {
        public string MarkingProject { get; set; }
        public string QIGName { get; set; }
        public int TotalScript { get; set; }
        public int CheckOutScripts { get; set; }
        public int TotalCompleted { get; set; }
        public int CompletionRateInPercentage { get; set; }
    }

    /// <summary>
    /// Candidate Script Model
    /// </summary>
    public class CandidateScriptModel
    {
        public string LoginName { get; set; }
        public string QIGName { get; set; }
        public string ScriptName { get; set; }
        public string status { get; set; }
        public long TotalRows { get; set; }
    }

    /// <summary>
    /// Candidate Script Model
    /// </summary>
    public class FIDIDetails
    {
        public long ProjectID { get; set; }
        public long QIGID { get; set; }
        public long ScheduleUserID { get; set; }
        public string LoginID { get; set; }
        public int status { get; set; }
        public long ProjectQuestionID { get; set; }
        public long QuestionID { get; set; }
        public string QuestionCode { get; set; }
        public decimal QuestionMaxMarks { get; set; }
        public int QuestionType { get; set; }
        public decimal TotalMarks { get; set; }
        public long ScoreComponentID { get; set; }
        public string ScoreComponentCode { get; set; }
        public string ScoreComponentName { get; set; }
        public decimal ComponentMaxMarks { get; set; }
        public decimal ComponentAwardedMarks { get; set; }
        public long ScriptID { get; set; }
        public bool IsNullResponse { get; set; }
        public decimal CandidateTotalMarks { get; set; }      
    }

    public class FIDIIdDetails
    {
        public FIDIIdDetails()
        {
            FIDIXAvgs = [];
            FIDIYAvgs = [];
            FIDIXYMuls = [];
            YvalWithoutX = [];
            subjectMarksPercentagelst = [];
            subjectMarksItemScoresModels = [];
        }
        public string ProductCode { get; set; }
        public string TNAQuestionCode { get; set; }
        public long ProjectID { get; set; }
        public long QIGID { get; set; }
        public long ProjectQuestionID { get; set; }
        public long ScoreComponentID { get; set; }
        public long SectionID { get; set; }
        public string SectionName { get; set; }
        public int QuestionType { get; set; }
        public string QuestionCode { get; set; }
        public string ComponentName { get; set; }
        public decimal QuestionMarks { get; set; }
        public decimal ComponentMarks { get; set; }
        public int TotalNoOfCandidates { get; set; }
        public int TotalCandidateMarks { get; set; }
        public decimal Mean { get; set; }
        public bool IsQIGLevel { get; set; }
        public decimal SD { get; set; }
        public decimal FI { get; set; }
        public decimal DI { get; set; }
        public decimal ItemMeanMark { get; set; }
        public decimal PercentTotScoredFullMark { get; set; }
        public List<FIDIXAvg> FIDIXAvgs { get; set; }
        public List<FIDIYAvg> FIDIYAvgs { get; set; }
        public List<FIDIXYMul> FIDIXYMuls { get; set; }
        public List<YMarkswithoutXscore> YvalWithoutX { get; set; }
        public List<SubjectMarksPercentageModel> subjectMarksPercentagelst { get; set; }
        public List<SubjectMarksItemScoresModel> subjectMarksItemScoresModels { get; set; }
        public decimal MaxMarks { get; set; }
        public decimal NullresponseMarks { get; set; }
        public decimal Nullresponsepercentage { get; set; }
        public decimal ZeroMarks { get; set; }
        public decimal Zeromarkspercentage { get; set; }
        public decimal OneMarks { get; set; }
        public decimal Onemarkspercentage { get; set; }
        public decimal twoMarks { get; set; }
        public decimal twomarkspercentage { get; set; }
        public decimal threeMarks { get; set; }
        public decimal threemarkspercentage { get; set; }
        public decimal fourMarks { get; set; }
        public decimal fourmarkspercentage { get; set; }
        public decimal fiveMarks { get; set; }
        public decimal fivemarkspercentage { get; set; }
        public decimal sixMarks { get; set; }
        public decimal sixmarkspercentage { get; set; }
        public decimal sevenMarks { get; set; }
        public decimal sevenmarkspercentage { get; set; }
        public decimal eightMarks { get; set; }
        public decimal eightmarkspercentage { get; set; }
        public decimal nineMarks { get; set; }
        public decimal ninemarkspercentage { get; set; }
        public decimal tenMarks { get; set; }
        public decimal tenmarkspercentage { get; set; }
        public decimal eleMarks { get; set; }
        public decimal elemarkspercentage { get; set; }
        public decimal twelMarks { get; set; }
        public decimal twelmarkspercentage { get; set; }
        public decimal thirtMarks { get; set; }
        public decimal thirtmarkspercentage { get; set; }
        public decimal fourtMarks { get; set; }
        public decimal fourtmarkspercentage { get; set; }
        public decimal fiftMarks { get; set; }
        public decimal fiftmarkspercentage { get; set; }
    }

    public class FIDIXAvg
    {
        public decimal xminusxbar { get; set; }
        public decimal xminusxbarsquare { get; set; }
    }
    public class FIDIYAvg
    {
        public decimal yminusybar { get; set; }
        public decimal yminusybarsquare { get; set; }
    }

    public class FIDIXYMul
    {
        public decimal xymulti { get; set; }
    }
    public class YMarkswithoutXscore
    {
        public decimal Yvalue { get; set; }
    }
    public class SubjectMarksPercentageModel
    {
        public decimal Marks { get; set; }
        public decimal Subpercentage { get; set; }        
    }

    public class SubjectMarksItemScoresModel
    {
        public decimal SubMarksitemsscores { get; set; }
        public decimal Marks { get; set; }
        public decimal Subpercentage { get; set; }
        public List<SubjectMarksPercentageModel> subjectMarksPercentagelst { get; set; }

    }


    public class FIDIReportModel
    {
        public FIDIReportModel()
        {
            fIDIDetails = new List<FIDIDetails>();
            fIDIIdDetails = new List<FIDIIdDetails>();
            SyncMetaData = new List<SyncMetaDataModel>();
        }
        public List<FIDIDetails> fIDIDetails { get; set; }
        public List<FIDIIdDetails> fIDIIdDetails { get; set; }

        public List<SyncMetaDataModel> SyncMetaData {  get; set; }
        public decimal MaxMarks { get; set; }
        public int MinMarks { get; set; }

    }
   
    public class SyncMetaDataModel
    {
        public string ExamLevel { get; set; }
        public string ExamSeries { get; set; }
        public string ExamYear { get; set; }
        public string Subject { get; set; }
        public string PaperNumber { get; set; }
        public List<QuestionValue> QuestionValues { get; set; }

        public SyncMetaDataResult syncresult { get; set; }
    }
    public class SyncMetaDataResult
    { 
       public string StatusCode {  get; set; }
        public string StatusMessage { get; set; }

        public static implicit operator string(SyncMetaDataResult v)
        {
            throw new NotImplementedException();
        }
    }

    public class QuestionValue
    {
        public string QuestionCode { get; set; }
        public double FI { get; set; }
        public double DI { get; set; }
    }

    /// <summary>
    /// Frequency Distribution Report
    /// </summary>
    public class FrequencyDistributionModel
    {
        public string MarkingProject { get; set; }
        public string QuestionCode { get; set; }
        public string Blank { get; set; }
        public string ResponseText { get; set; }
        public int NoOfCandidatesAnswered { get; set; }
        public string Responses { get; set; }
        public decimal PercentageDistribution { get; set; }
        public long QuestionId { get; set; }
        public long QuestionType { get; set; }
        public decimal MarksAwarded { get; set; }
        public string MarkingType { get; set; }
        public string Remarks { get; set; }
        public long RowNumbers { get; set; }
        public long TotalRows { get; set; }
        public string status { get; set; }
        public List<Automatic> automatics { get; set; }
    }
    public class Automatic
    {
        public long QuestionId { get; set; }
        public string ChoiceText { get; set; }
        public long MaxScore { get; set; }
        public bool IsCorrect { get; set; }
        public string ChoiceIdentifier { get; set; }
    }

    /// <summary>
    /// Get All Project Model
    /// </summary>
    public class BindAllProjectModel
    {
        public long ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public bool IsProjectSame { get; set; }
    }

    /// <summary>
    /// Gets and Sets all AnswerKeys
    /// </summary>
    public class AllAnswerKeysModel
    { 
        public string ParentQuestionCode { get; set; }
        public string QuestionCode { get; set; }
        public string ChoiceText { get; set; }
        public int QuestionType { get; set; }
        public string QuestionName { get; set; }
        public int QuestionOrder { get; set; }
        public string status { get; set; }
        public string OptionText { get; set; }
        public long TotalCount { get; set; } 
        public long QuestionMarks { get; set; }

    }

    public class Mailsentdetails
    {
        public string UserName { get; set; }
        public string LoginName { get; set; }
        public string Role { get; set; }
        public string RoleCode { get; set; }
        public string NRIC { get; set; }
        public bool IsActive { get; set; }
        public bool IsMailSent { get; set; }
        public DateTime? MailSentDate { get; set; }
        public long TotalCount { get; set; }
    }

    public class ClsMailSent
    {
        public long ProjectUserRoleID { get; set; }
        public string Role { get; set; }
        public string School { get; set; }
        public string SearchText { get; set; }
        public string Status { get; set; }
        public int PageSize { get; set; }
        public int PageNo { get; set; }
        public int SortOrder { get; set; }
        public string SortField { get; set; }
        public int IsEnabled { get; set; }

        public UserTimeZone UserTimeZone { get; set; }

    }

    /// <summary>
    /// Get All Project Model
    /// </summary>
    public class BindAllMarkerPerformanceModel
    {
        public long ID { get; set; }
        public long ProjectId { get; set; }
        public string ProjectCode { get; set; }
        public string ProjectName { get; set; }
        public string RoleCode { get; set; }
        public long ReMarkedScripts { get; set; }
        public string AverageTime { get; set; }
        public string TotalTimeTaken { get; set; }
        public long NoOfScripts { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public long ProjectUserRoleID { get; set; }
        public string MarkerName { get; set; }
        public string status { get; set; }
        public long RowCounts { get; set; }
        public long RowNum { get; set; }
    }

}
