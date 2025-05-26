using Saras.eMarking.Domain.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Project.Setup.ResolutionOfCoi
{
    public class ResolutionOfCoiModel
    {
        public ResolutionOfCoiModel()
        {
        }
        [XssTextValidation]
        public string UserName { get; set; }
        [XssTextValidation]
        public string RoleName { get; set; }
        [XssTextValidation]
        public string SendingSchoolName { get; set; }
        [XssTextValidation]
        public string SendingSchoolCode { get; set; }
        [Required]
        public long ProjectUserRoleID { get; set; }
        [Required]
        public long USERID { get; set; }
        public int? SendingSchoolID { get; set; }
        public List<CoiSchoolModel> SchoolList { get; set; }
    }
}
