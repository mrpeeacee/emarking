using Saras.eMarking.Domain.Extensions;

namespace Saras.eMarking.Domain.ViewModels
{
    public class QuestionBandModel
    {

        public long BandId { get; set; }
        [XssTextValidation]
        public string BandCode { get; set; }
        [XssTextValidation]
        public string BandName { get; set; }
        public long ProjectQuestionId { get; set; }
        [XssTextValidation]
        public string QuestionCode { get; set; }
        [XssTextValidation]
        public string QuestionText { get; set; }
    }
}
