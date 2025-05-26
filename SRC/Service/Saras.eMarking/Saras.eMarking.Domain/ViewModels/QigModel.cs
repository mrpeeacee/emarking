using Saras.eMarking.Domain.Extensions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels
{
   
    public class QigModel
    {
        public QigModel()
        {
        }
        [Required]
        public long QigId { get; set; }
        [Required]
        [XssTextValidation]
        public string QigName { get; set; }
        [Required]
        public RandomCheckType RcType { get; set; }
        public List<ProjectTeamsIdsModel> TeamIds { get; set; }
        public List<AppSettingModel> RandomCheckSettings { get; set; }
        public List<AppSettingModel> AnnotationSetting { get; set; }
    }
}
