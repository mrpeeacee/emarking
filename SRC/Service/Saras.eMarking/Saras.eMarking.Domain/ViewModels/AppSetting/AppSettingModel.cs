using Saras.eMarking.Domain.Entities;
using Saras.eMarking.Domain.Extensions;
using Saras.eMarking.Domain.ViewModels.AuditTrail;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 

namespace Saras.eMarking.Domain.ViewModels
{
    public class AppSettingModel : IAuditTrail
    {
        [Required]
        public long? SettingGroupID { get; set; }
        [XssTextValidation]
        public string SettingGroupCode { get; set; }
        [XssTextValidation]
        public string SettingGroupName { get; set; }
        [XssTextValidation]
        public string AppsettingKey { get; set; }
        [XssTextValidation]
        public string AppsettingKeyName { get; set; }
        public long? ParentAppsettingKeyID { get; set; }
        public long? EntityID { get; set; }
        public EnumAppSettingEntityType EntityType { get; set; }
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "Please enter a value bigger than {1}")]
        public long AppSettingKeyID { get; set; }
        [XssTextValidation]
        public string Value { get; set; }
        [XssTextValidation]
        public string DefaultValue { get; set; }
        public EnumAppSettingValueType ValueType { get; set; }
        public long? ProjectID { get; set; }
        public int? ProjectStatus { get; set; }
        public List<AppSettingModel> Children { get; set; }
    }
}
