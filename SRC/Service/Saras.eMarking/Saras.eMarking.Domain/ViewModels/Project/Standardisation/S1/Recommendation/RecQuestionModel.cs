using Saras.eMarking.Domain.Extensions;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Recommendation
{
    public class RecQuestionModel
    {
        public RecQuestionModel()
        {

        }
        public long ScriptId { get; set; }
        public  string ScriptName { get; set; } 
        public string FirstName {  get; set; }
        public string CurrentUserRoleCode {  get; set; }
        public long? ProjectQnsId { get; set; }
        public int? TotalNoOfQuestions { get; set; }
        [XssTextValidation]
        public string QuestionCode { get; set; }
        [XssTextValidation]
        public string QuestionText { get; set; }
        public int? QuestionOrder { get; set; }
        public QuestionResponseType ResponseType { get; set; }
        [XssTextValidation]
        public string ResponseText { get; set; }
        public RecBandModel RecommendedBand { get; set; }
        public List<RecBandModel> Bands { get; set; }
        [JsonIgnore]
        public long? RecommendedBandId { get; set; }
        public long? ScheduleUserID { get; set; }
        public long? UserID { get; set; }
       
        public Nullable<decimal> FinalizedMarks { get; set; }
        
        public Nullable<decimal> QuestionMarks { get; set; }
        public long? ProjectUserQuestionResponseID { get; set; }
        public bool IsQigLevel { get; set; }
        public bool IsMarkSchemeTagged { get; set; }
        public bool IsScoreComponentExists { get; set; }
        public decimal? StepValue { get; set; }

        public long? QuestionID { get; set; }
        public string QuestionXML { get; set; }

        public string Correctanswers { get; set; }
        public string QuestionGUID { get; set; }
        public long? PassageID { get; set; }
        public string PassageVersion { get; set; }

        public long? Markedtype{ get; set; }
        public string PassageText { get; set; }
        public string PassageXML { get; set; }
        public decimal? Questionversion { get; set; }

        public long? QuestionType { get; set; }

        public decimal? TotalMarks { get; set; }
        public int NoofMandatoryQuestion { get; set; }
        public bool? IsNullResponse { get; set; }
        public bool? TestPlayerView { get; set; }

    }

    public class QigRecommendationAction : IAuditTrail
    {
        public QigRecommendationAction()
        {
            ScriptResponses = new List<RecQuestionModel>();
        }
        public List<RecQuestionModel> ScriptResponses { get; set; }
        public long ScriptId { get; set; }
        public long Qigid { get; set; }
    }

    public class UnrecommandedScript 
    {
        public long ScriptId { get; set; }
        public long Qigid { get; set; }
        public string RoleCode { get; set; }
    }


    public class SoreFingerResponse
    {
        public string Identifier { get; set; }
        public string AnsweredWord { get; set; }

        public string MarkedWord { get; set; }
    }

    public class SoreFingerQuestion
    {
        public string inlineStatic { get; set; }
        public string hottext { get; set; }
    }

}
