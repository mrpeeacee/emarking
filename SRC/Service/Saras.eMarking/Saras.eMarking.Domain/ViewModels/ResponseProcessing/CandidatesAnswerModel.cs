using Saras.eMarking.Domain.Extensions;
using System;

namespace Saras.eMarking.Domain.ViewModels.ResponseProcessing
{
    public class CandidatesAnswerModel
    {
        public CandidatesAnswerModel()
        {

        }
        [XssTextValidation]
        public string CandidatesAnswer { get; set; }
        public int? Responses { get; set; }
        public decimal? PerDistribution { get; set; }
        public decimal? MarksAwarded { get; set; }
        public int? MarkingType { get; set; }
        public Int64 Id { get; set; }
        public decimal? MaxMarks { get; set; }
        public long? QigId { get; set; }
        public bool Standardization { get; set; }
        public long ProjectQuestionId { get; set; }
        public bool IsCorrectAnswer { get; set; }
        [XssTextValidation]
        public string Remarks { get; set; }
        public string QuestionCode { get; set; }
        public string ResponseText { get; set; }
        public bool? IsDiscrepancyExist { get; set; }
        public int? DiscrepancyStatus { get; set; }

    }
}
