using Saras.eMarking.Domain.Extensions;
using System;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels.Report
{
    public class ScheduleDetailsModel
    { 
        public string AssessmentId { get; set; }
        public string ScheduleUserId { get; set; }
        public string SectionId { get; set; }
        public string Solution { get; set; }
        public string UserId { get; set; }
        public string Theme { get; set; }
        public string SumType { get; set; }

        public string TestType { get; set; }
        public string Page { get; set; }
        public string culture { get; set; }
        public string Key { get; set; }
        public string TestMode { get; set; }
        public string TimeStamp { get; set; }
        public string Url { get; set; }
    }
    public class UserQuestionResponse
    {
        public long ProjectQuestionId { get; set; }
        public long QuestionID { get; set; }
        public string QuestionXml { get; set; }
        public int? QuestionType { get; set; }
        public string QuestionCode { get; set; }
        public long? ParentQuestionId { get; set; }
        public bool IsChildExist { get; set; }
        public string ResponseText { get; set; }
        public string QuestionText { get; set; }
        public string QuestionTextReport { get; set; }
        public string candidateindex { get; set; }

        public long? UserId { get; set; }
        public List<Choice> Choices { get; set;}

        public long TotalRows{get;set;}
    }
    public class Choice
    {

        [XssTextValidation]
        public string OptionText { get; set; }
    }
    public class AllUserQuestionResponses
    {
        public string candidateindex { get; set; }
        public List<UserQuestionResponse> UserQuestionResponses { get; set; }
    }

    public class SchoolInfoDetails
    {
        public long SchoolID { get; set; }
        public string SchoolName { get; set; }
        public string SchoolCode { get; set; }
        public bool IsSchoolSelected { get; set; }
    }
}
