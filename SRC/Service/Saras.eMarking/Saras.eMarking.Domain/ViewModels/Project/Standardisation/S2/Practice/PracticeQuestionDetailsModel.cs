using Saras.eMarking.Domain.Extensions;

namespace Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdTwo.Practice
{
    public class PracticeQuestionDetailsModel
    {
        public PracticeQuestionDetailsModel()
        {

        }
        public long QuestionID { get; set; }
        [XssTextValidation]
        public string QuestionLabel { get; set; }
        public decimal? TotalMarks { get; set; }
        public decimal? DefenetiveMarks { get; set; }
        public decimal? AwardedMarks { get; set; }
        public int? ToleranceLimit { get; set; }
        public bool? IsOutOfTolerance { get; set; }
        public int? QuestionType { get; set; }
    }
}
