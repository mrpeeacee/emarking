using Saras.eMarking.Domain.Extensions;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Setup
{
    public class ExamCenterModel
    {
        [Required]
        public long ProjectCenterID { get; set; }
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long ProjectID { get; set; }
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long CenterID { get; set; }
        [XssTextValidation]
        public string CenterName { get; set; }
        [XssTextValidation]
        public string CenterCode { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long? TotalNoOfScripts { get; set; }
        public bool? IsSelectedForRecommendation { get; set; }
        public bool? IsRecommended { get; set; }
        public int noresponsecount { get; set; }
    }

    public class ExamCenterActionModel : IAuditTrail
    {
        public ExamCenterActionModel()
        {
            ExamCenterModel = new List<ExamCenterModel>();
        }
        public List<ExamCenterModel> ExamCenterModel { get; set; }
        public long QigId { get; set; }
    }
}
