using Saras.eMarking.Domain.Extensions;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Project.Setup
{
    public class BasicDetailsModel : IAuditTrail
    {
        [XssTextValidation]
        public string ProjectName { get; set; }
        public long ProjectID { get; set; }
        [XssTextValidation]
        public string ProjectCode { get; set; }
        [XssTextValidation]
        public string SubjectName { get; set; }
        [XssTextValidation]
        public string SubjectCode { get; set; }
        [XssTextValidation]
        public string PaperName { get; set; }
        public short ExamYear { get; set; }
        [XssTextValidation]
        public string ExamSeriesName { get; set; }
        public string ExamLevelName { get; set; }
        [XssTextValidation]
        public string MOAName { get; set; }
        [MaxLength(2000)]
        [XssTextValidation]
        public string ProjectInfo { get; set; }
    }

    public class GetModeOfAssessmentModel
    {
        public string MOACode { get; set; }
    }
}
