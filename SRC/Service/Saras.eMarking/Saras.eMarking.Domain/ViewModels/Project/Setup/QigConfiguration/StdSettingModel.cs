using Saras.eMarking.Domain.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration
{
    public class StdSettingModel
    {
        public StdSettingModel()
        {
        }

        public long? SettingID { get; set; }
        public long? QIGID { get; set; }
        [XssTextValidation]
        public string QIGCode { get; set; }
        [XssTextValidation]
        public string QIGName { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int? StandardizationScript { get; set; }
        [Required]
        //[Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int? BenchmarkScript { get; set; }
        [Required]
        //[Range(1, int.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public int? AdditionalStdScript { get; set; }
        public int? QualityAssuranceScript { get; set; }
        [Required]
        public bool Isdeleted { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        [Required]
        public bool? IsS1Available { get; set; }
        [Required]
        public bool? IsS2Available { get; set; }
        [Required]
        public bool? IsS3Available { get; set; }
        public Boolean IsPracticemandatory { get; set; }
        public DateTime? S1StartDate { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long? RecomendationPoolCount { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long? RecomendationPoolCountPerKP { get; set; } 
        [XssTextValidation]
        public string RecommendationPoolCountAppSettingKey { get; set; }
        [XssTextValidation]
        public string RecommendationCountKPAppSettingKey { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long? AppSettingKeyIDPoolCount { get; set; }
        public long? AppSettingKeyIDPoolCountPerKP { get; set; }
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        [XssTextValidation]
        public string Ispauseoronholdors1comp { get; set; }
        [Required]
        public Boolean IsRecommandedQig { get; set; }
        [Required]
        public Boolean IsPracticed { get; set; }
        [Required]
        public Boolean Disablestdreq { get; set; }
        [Required]
        public Boolean Offstdreq { get; set; }
        [Required]
        public Boolean Disablestandardisationreq { get; set; }
        public Boolean IsS1ClosureCompleted { get; set; }
        [XssTextValidation]
        public string RecommendMarkScheme { get; set; }
        [Required]
        public Boolean IsScriptRecommended { get; set; }
        public bool? IsLiveMarkingStart { get; set; }
    }
}
