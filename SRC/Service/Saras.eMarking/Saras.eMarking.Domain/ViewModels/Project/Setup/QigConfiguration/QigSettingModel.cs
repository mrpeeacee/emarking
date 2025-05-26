using Saras.eMarking.Domain.Extensions;
using System;
using System.ComponentModel.DataAnnotations;

namespace Saras.eMarking.Domain.ViewModels.Project.Setup.QigConfiguration
{
    public class QigSettingModel
    {
        public QigSettingModel()
        {
        }
        [XssTextValidation]
        public string GracePeriod { get; set; }
        [XssTextValidation]
        public string DownloadBatchSize { get; set; }
        [XssTextValidation]
        public string MarkingType { get; set; }
        [Required]
        public Int16 StepValue { get; set; }
        [XssTextValidation]
        public string RecommendedMarkScheme { get; set; }
        [Required]
        public Boolean ExceedDailyQuotaLimit { get; set; }
        [Required]
        public Boolean IsQiGClosureEnabled { get; set; }
        [XssTextValidation]
        [MaxLength(150)]
        public string QiGClosureRemarks { get; set; }
        public Boolean IsPauseMarkingProcessEnabled { get; set; }
        [XssTextValidation]
        [MaxLength(150)]
        public string PauseMarkingProcessRemarks { get; set; }
        [Required]
        public Boolean IsAnnotationsMandatory { get; set; }
        [Required]
        public Boolean IsScriptTrialMarked { get; set; }
        [Required]
        public Boolean IsScriptRecommended { get; set; }
        public byte ProjectStatus { get; set; }
    }

    public class LiveMarkingDailyQuotaModel
    {
        public int DailyQuota { get; set; }
        public long QigId { get; set; }
    }
}
