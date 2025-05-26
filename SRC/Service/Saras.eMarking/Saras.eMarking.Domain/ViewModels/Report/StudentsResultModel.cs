using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels.Report
{
    public class StudentsResultStatistics
    {
        public decimal TotalStudentsCount { get; set; }
        public decimal TotalSchoolCount { get; set; }
        public decimal TotalMarks { get; set; }
    }
    public class StudentsResult
    {
        public string StudentId { get; set; }
        public string SchoolName { get; set; }
        public decimal MaxMark { get; set; }
        public decimal SecuredMark { get; set; }
        public bool Result { get; set; }
    }
    public class StudentwiseReportModel
    {
        public string QigName { get; set; }
        public string QigCode { get; set; }
        public List<StudentwiseQuestions> Questions { get; set; }

    }
    public class StudentwiseQuestions
    {
        public string QuestionNo { get; set; }
        public string QuestionCode { get; set; }
        public decimal MaxMarks { get; set; }
        public decimal SecuredMarks { get; set; }

    }
    public class ParamStdDetails
    {
        public string StudentId { get; set; }
        public string SchoolCode { get; set; }
        public decimal Markfrom { get; set; }
        public decimal MarkTo { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class CourseValidationModel
    {
        public string Scheduleid { get; set; }
        public string ScheduleName { get; set; }
        public string ProjectName { get; set; }
        public bool IsExamClosed { get; set; }
        public string JobStatus { get; set; }
        public DateTime? ProjectCreatedDate { get; set; }
        public bool IsMarkPersonImported { get; set; }
        public bool IsScriptsImported { get; set; }
        public bool IsReadyForEmarkingProcess { get; set; }
        public bool ProjectCreated { get; set; }
    }
    public class StudentcompleteScriptReport
    {
        public string Index_Number { get; set; }
        public string ScheduleName { get; set; }
        public string CandidateTotalMarks { get; set; }
        public string QuestionCode { get; set; }
        public string Identifier { get; set; }
        public string QuestionOrder { get; set; }
        public string TotalMarks { get; set; }
        public string Score { get; set; }
        public string Score_Type { get; set; }
        public string Score_Status { get; set; }
        public string ProjectCode { get; set; }
        public string Comments { get; set; }
        public string ModifiedOn { get; set; }
        public string PHASE { get; set; }
        public string Scriptstatus { get; set; }
        public string Reason { get; set; }
        public string Score_Components { get; set; }
        public string USERNAME { get; set; }
        public string TestPackage {  get; set; }
        public string ItemName { get; set; }
        public string MarkingProject { get; set; }
        public string ROLENAME { get; set; }
        public string MARKER_SENIORITY { get; set; }
        public string MARKER_NAME { get; set; }
        public string MARKER_USERNAME { get; set; }
        public string AssetSubTypeName { get; set; }
        public string AnswerResponse { get; set; }
        public string AnswerType { get; set; }
        public string DisplayResponse { get; set; }
        public string Minutes_Taken_To_Mark { get; set; }

    }

    public class XmlToString
    {
        public string QuestionID { get; set; }
        public string ChoiceText { get; set; }
        public string ChoiceGUID { get; set; }
        public string Choice { get; set; }
        public string QuestionCode { get; set; }
        public string IsCorrect { get; set; }
        public string OptionText { get; set; }
    }



}
