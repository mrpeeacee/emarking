using Saras.eMarking.Domain.Extensions;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels.ResponseProcessing
{
    public class AutomaticQuestionsModel
    {
        public AutomaticQuestionsModel()
        {

        }
        public int? QuestionType { get; set; }
        public bool? GlobalMarkingType { get; set; }
        [XssTextValidation]
        public string QuestionText { get; set; }
        public string QuestionXML { get; set; }
        public long ProjectQuestionId { get; set; }
        [XssTextValidation]
        public string QuestionCode { get; set; }
        public long? QuestionId { get; set; }
        public decimal? QuestionVersion { get; set; }
        public decimal? QuestionMarks { get; set; }
        public NotRespondedChoice NotResponsded { get; set; }
        public string status { get; set; }
        public bool? IsQuestionXMLExist { get; set; }
        public List<OptionArea> optionAreas { get; set; }
        public List<GuidOptionArea> guidoptionAreas { get; set; }
        public ChoiceList[] ChoiceList { get; set; }
        public bool? Isqigreset { get; set; }
        [XssTextValidation]
        public string QuestionGUID { get; set; }

        public string PassageXML { get; set; }
        public string PassageText { get; set; }

        public long? PassageId { get; set; }

        public MatrixQuestion MatrixQuestions { get; set; }
    }
   
    public class MatrixQuestion
    {
        public string QuestionText { get; set; }
        public Dictionary<string, string> Rows { get; set; } // Row ID to Row Text
        public Dictionary<string, string> Columns { get; set; } // List of Column Texts
        public Dictionary<string, string> CorrectAnswers { get; set; } // Row ID to Correct Option ID

        public string DisplayHeader { get; set; }
    }



    public class ChoiceList
    {
        public List<Choice> Choices { get; set; }
        public string Blank { get; set; }
        public NotRespondedChoice NotResponsded { get; set; }
    }
    public class Choice
    {

        [XssTextValidation]
        public string OptionText { get; set; }
        [XssTextValidation]
        public string CandidatesAnswer { get; set; }
        public int? NoOfCandidates { get; set; }
        public int? NoOfCandidatesAnswered { get; set; }
        public int? Responses { get; set; }
        public bool IsCorrectAnswer { get; set; }
        public decimal? PerDistribution { get; set; }
        public string ChoiceIdentifier { get; set; }
        public long ProjectQuestionId { get; set; }
        public int? markingType { get; set; }
        [XssTextValidation]
        public string Remarks { get; set; }

    }
    public class NotRespondedChoice
    {
        [XssTextValidation]
        public string NoOptionText { get; set; }
        [XssTextValidation]
        public string NoRespChoiceIdentifier { get; set; }
        public int? NoOfCandidatesNotAnswered { get; set; }
        public decimal? NoresponsePerDistribution { get; set; }
    }
}
