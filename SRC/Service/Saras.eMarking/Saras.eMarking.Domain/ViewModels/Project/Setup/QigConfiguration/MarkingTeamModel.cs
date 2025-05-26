using Saras.eMarking.Domain.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration
{
    public class MarkingTeamModel
    {
        public MarkingTeamModel()
        {
        }
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long ProjectUserRoleID { get; set; }
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long UserID { get; set; }
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long RoleID { get; set; }
        [XssTextValidation]
        [Required]
        public string RoleCode { get; set; }
        [XssTextValidation]
        [Required]
        public string RoleName { get; set; }
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long ReportingTo { get; set; }
        [XssTextValidation]
        [Required]
        public string UserName { get; set; }
        [XssTextValidation]
        [Required]
        public string AoName { get; set; }
        [XssTextValidation]
        [Required]
        public string CmName { get; set; }
        [XssTextValidation]
        [Required]
        public string AcmName { get; set; }
        [XssTextValidation]
        [Required]
        public string TlName { get; set; }
        [XssTextValidation]
        [Required]
        public string AtlName { get; set; }
        [XssTextValidation]
        [Required]
        public string MarkerName { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int IsNewRow { get; set; }
    }
}
