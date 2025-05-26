using Saras.eMarking.Domain.Extensions;
using System.Collections.Generic;

namespace Saras.eMarking.Domain.ViewModels.ResponseProcessing
{
    public class ViewFrequencyDistributionModel
    {
        public ViewFrequencyDistributionModel()
        {

        }
        public decimal? TotalMarks { get; set; }
        public long NoOfBlanks { get; set; }
        public long NoOfCandidates { get; set; }
        [XssTextValidation]
        public string QuestionsText { get; set; }
        public string QIGName { get; set; }
        public string QIGCode { get; set; }
        public long QIGId { get; set; }
        public long ProjectQuestionId { get; set; }
        public long QuestionId { get; set; }
        [XssTextValidation]
        public string QuestionGUID { get; set; }
        [XssTextValidation]
        public string QuestionCode { get; set; }

        public List<BlankOptionModel> BlankOption { get; set; }
        public int? QuestionOrder { get; set; }
        public bool IsDiscrepancyExist { get; set; }
        public bool IsCaseSensitive { get; set; }
        public long? ParentQuestionId { get; set; }
        public int? ResponseProcessingType { get; set; }

    }
    public class BlankOptionModel
    {
        [XssTextValidation]
        public string CorrectAnswer { get; set; }
        public decimal? BlankMarks { get; set; }
        public long ProjectQuestionId { get; set; }
        public List<CandidatesAnswerModel> CandidateAnswer { get; set; }
        public string QIGName { get; set; }
        public string QIGCode { get; set; }
        public long QIGId { get; set; }
        public string QuestionGUID { get; set; }
        public bool IsManuallyMarkEnabled { get; set; }
        public bool IsCaseSensitive { get; set; }
        public long? ParentQuestionId { get; set; }
        public int? ResponseProcessingType { get; set; }
    }
}
