using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Saras.eMarking.Domain.Extensions;
using Saras.eMarking.Domain.ViewModels.AuditTrail;

namespace Saras.eMarking.Domain.ViewModels
{
    public class QigQuestionModel
    {
        [XssTextValidation]
        public string QuestionCode { get; set; }
        public long QigId { get; set; }
        public string XmlQuestiopns { get; set; }
        public int? QuestionType { get; set; }
        public int? QigType { get; set; }
        public Boolean IsS1ClosureCompleted { get; set; }

        public decimal? MaxMark { get; set; }
        public int ToleranceLimit { get; set; }
        [Required]
        public bool IsScoreComponentExists { get; set; }
        public decimal? StepValue { get; set; }
        [XssTextValidation]
        public string QuestionText { get; set; }
        public long? QuestionId { get; set; }
        public long? ProjectQuestionID { get; set; }
        public long? MarkSchemeId { get; set; }
        public string MarkSchemeName { get; set; }
        public string SqurebraQuesId { get; set; }

        public long ScoringComponentLibraryId { get; set; }

		public List<ScoreComponentDetails> Scorecomponentdetails { get; set; }
        public bool IsActive { get; set; }
        public string QuestionXML { get; set; }

        public string PassageXML { get; set; }
        public long? PassageId { get; set; }
        public bool IsChecked { get; set; }
        public decimal? QuestionVersion { get; set; }
        public decimal PassageVersion { get; set; }
        [XssTextValidation]
        public string PassageText { get; set; }
        public bool? IsTrialmarkedorcategorised { get; set; }
        public string BlankText { get; set; }
        public string status { get; set; }
        public bool? IsQuestionXMLExist { get; set; }
        public int? QuestionOrder { get; set; }
        public bool? IsDiscrepancyExist { get; set; }
        public bool? MarkingType { get; set; }
        public string QuestionGUID { get; set; }
        public long? noOfQuestions { get; set; }
        public long? noOfMandatoryQuestion { get; set; }
        public List<OptionArea> optionAreas { get; set; }
        public bool? Isqigreset { get; set; }
        public long? DiscrepancyStatus { get; set; }
    }

    public class ScoreComponentDetails
    {
        public long ScoreComponentId { get; set; }
        public decimal MaxMark { get; set; }
        public string ComponentCode { get; set; }
        public string ComponentName { get; set; }
        public long? CompMarkSchemeId { get; set; }
        public string SchemeName { get; set; }
        public long? ProjectQuestionId { get; set; }
        public long? ProjectMarkSchemeId { get; set; }
        public bool IsChecked { get; set; }
        public bool IsAutoCreated { get; set; }

        public long ScoreComponentLibID { get; set; }

	}
    public class OptionArea
    {
        [XssTextValidation]
        public string OptionAreaName { get; set; }


        [XssTextValidation]
        public string OptionText { get; set; }
        [XssTextValidation]
        public string CandidatesAnswer { get; set; }
        public int? NoOfCandidates { get; set; }
        public int? NoOfCandidatesAnswered { get; set; }
        public int? Responses { get; set; }
        public decimal? PerDistribution { get; set; }
        public long ProjectQuestionId { get; set; }
        public int? markingType { get; set; }
        [XssTextValidation]
        public string QuestionGUID { get; set; }
    }

    public class GuidOptionArea
    {
        [XssTextValidation]
        public string QuestionGUID { get; set; }
        [XssTextValidation]
        public string OptionAreaName { get; set; }
    }
    public class UpdatedQigQuestions: IAuditTrail
    {

        public long projectQuestionId { get; set; }

        public long questionMaxmarks { get; set; }

        public string QuestionCode { get; set; }

    }
    public class ScoringComponent
    {

        public long ScoreComponentId { get; set; }

		public String ComponentCode { get; set; }
		public String ComponentName { get; set; }
		public decimal? Marks { get; set; }
		public bool IsTagged { get; set; }
		public bool IsActive { get; set; }

		public bool IsDeleted { get; set; }


	}

}
