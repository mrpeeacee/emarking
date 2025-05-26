using System;
using System.Collections.Generic;
using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Extensions;

namespace Saras.eMarking.Domain.ViewModels
{
    public class UserScriptResponseModel
    {
        public UserScriptResponseModel()
        {
        }

        public long? ProjectQuestionId { get; set; }
        public long? ScriptId { get; set; }
        [XssTextValidation]
        public string ScriptName { get; set; }
        public int? TotalNoOfQuestions { get; set; }
        public int? TotalNoOfResponses { get; set; }
        [XssTextValidation]
        public string QIGName { get; set; }
        public Nullable<decimal> QuestionMarks { get; set; }
        public long? userscriptID { get; set; }
        public long? ProjectUserQuestionResponseID { get; set; }
        [XssTextValidation]
        public string Annotation { get; set; }
        [XssTextValidation]
        public string ImageBase64 { get; set; }
        [XssTextValidation]
        public string Comments { get; set; }
        public long? Workflowstatusid { get; set; }
        public string ResponseText { get; set; }
        public byte? MarkedType { get; set; }
        public long? BandID { get; set; }
        public Nullable<decimal> Marks { get; set; }
        public Nullable<bool> Lastvisited { get; set; }
        public Nullable<decimal> FinalizedMarks { get; set; }
        public bool IsScoreComponentExists { get; set; }
        public byte? MarkingStatus { get; set; }

        public string QuestionCode { get; set; }
        
        public string Remarks { get; set; }
        public decimal? TotalMarks { get; set; }
        public int NoofMandatoryQuestion { get; set; }
        public decimal? awardedmarks { get; set; }
        public bool? IsAutoSave { get; set; }
        public List<QuestionScoreComponentMarkingDetail> ScoreComponentMarkingDetail { get; set; }
}


}
