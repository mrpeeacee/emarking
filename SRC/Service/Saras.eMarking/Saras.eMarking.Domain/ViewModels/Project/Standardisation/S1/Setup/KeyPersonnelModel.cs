using Saras.eMarking.Domain.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Setup
{
    public class KeyPersonnelModel
    {
        [Required]
        public long ProjectUserRoleID { get; set; }
        [Required]
        public long RoleID { get; set; }
        [Required]
        [XssTextValidation]
        public string RoleCode { get; set; }
        [XssTextValidation]
        public string RoleName { get; set; }
        [Required]
        [XssTextValidation]
        public string LoginName { get; set; }
        public bool? IsKP { get; set; }
        public bool? IsKpTagged { get; set; }
        public bool? IsKpTrialmarkedorcategorised { get; set; }
    }
}
