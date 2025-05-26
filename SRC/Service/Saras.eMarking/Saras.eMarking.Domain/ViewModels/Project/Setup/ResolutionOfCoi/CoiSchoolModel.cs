using Saras.eMarking.Domain.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Project.Setup.ResolutionOfCoi
{
    public class CoiSchoolModel
    {
        public CoiSchoolModel()
        {
        }
        [XssTextValidation]
        public string SchoolName { get; set; }
        [XssTextValidation]
        public string SchoolCode { get; set; }
        public int? ExemptionSchoolID { get; set; }
        [Required]
        public Boolean IsSendingSchool { get; set; }
        [Required]
        public int SchoolID { get; set; }
        [Required]
        public Boolean IsSelectedSchool { get; set; }
        [Required]
        public int ProjectId { get; set; }
    }
}
