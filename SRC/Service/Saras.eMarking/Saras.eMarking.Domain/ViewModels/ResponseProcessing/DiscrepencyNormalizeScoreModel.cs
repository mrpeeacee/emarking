using Saras.eMarking.Domain.Extensions;

namespace Saras.eMarking.Domain.ViewModels.ResponseProcessing
{
    public class DiscrepencyNormalizeScoreModel
    {
        public DiscrepencyNormalizeScoreModel()
        {

        }
        public decimal? MarksAwarded { get; set; }
        public long ProjectQuestionID { get; set; }
        [XssTextValidation]
        public string ResponseText { get; set; }
        public decimal? QuestionMarks { get; set; }
        public long QigId { get; set; }
        public long Id { get; set; }
        public long?[] ScriptIds { get; set; }
    }
}
