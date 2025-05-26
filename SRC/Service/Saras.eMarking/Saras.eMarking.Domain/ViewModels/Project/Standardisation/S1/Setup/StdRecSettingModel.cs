using Saras.eMarking.Domain.Extensions;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Project.Standardisation.StdOne.Setup
{
    public class StdRecSettingModel
    {
        public long? QIGID { get; set; }
        [XssTextValidation]
        public string QIGCode { get; set; }
        [XssTextValidation]
        public string QIGName { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long? AppSettingKeyIDPoolCount { get; set; }
        public long? AppSettingKeyIDPoolCountPerKP { get; set; }
        [XssTextValidation]
        public string RecommendationPoolCountAppSettingKey { get; set; }
        [XssTextValidation]
        public string RecommendationCountKPAppSettingKey { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long? RecomendationPoolCount { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long? RecomendationPoolCountPerKP { get; set; }
        public long? script_total { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        [XssTextValidation]
        public string Ispauseoronholdors1comp { get; set; }
    }      
}
