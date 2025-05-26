using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration.Annotation
{
    public class AnnotationSettingModel
    {
        public AnnotationSettingModel()
        {
        } 
        [Required]
        public bool IsAnnotationsMandatory { get; set; }
        [Required]
        public bool IsScriptTrialMarked { get; set; }
        public bool IsTagged { get; set; }
    }
}
