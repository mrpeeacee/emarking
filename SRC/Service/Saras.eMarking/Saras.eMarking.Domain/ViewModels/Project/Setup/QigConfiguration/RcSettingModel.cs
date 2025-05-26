using Saras.eMarking.Domain.Extensions;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 

namespace Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration
{
    public class RcSettingModel : IAuditTrail
    {
        public RcSettingModel()
        {
        }
        [Required]
        public long QigId { get; set; }
        public bool IsLiveMarkingEnabled { get; set; }
        
        public long TotalLivePoolScriptCount { get; set; }
        public long LivePoolCount { get; set; }
        public long SubmittedCount { get; set; }
        public long RC1SubmittedCount { get; set; }
        public long RC1SelectedCount { get; set; }
        public long RC2SelectedCount { get; set; }
        public long FinalisedScriptCountLiveMarking { get; set; }
        public long FinalisedScriptCountRC2 { get; set; }

        [Required]
        [XssTextValidation]
        public string QigName { get; set; }
        [Required]
        public RandomCheckType RcType { get; set; }
        public List<AppSettingModel> RandomCheckSettings { get; set; }
    }
}
