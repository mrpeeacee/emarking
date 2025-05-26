using Saras.eMarking.Domain.Extensions;

namespace Saras.eMarking.Domain.ViewModels
{
    public class AppSettingKeyModel
    {
        public AppSettingKeyModel()
        {
        }
        public long AppsettingKeyID { get; set; }
        [XssTextValidation]
        public string AppsettingKey { get; set; }
        [XssTextValidation]
        public string AppsettingKeyName { get; set; }
        [XssTextValidation]
        public string Description { get; set; }
        public int? SettingGroupID { get; set; }
        public long? ParentAppsettingKeyID { get; set; }
        public long? OrganizationID { get; set; }
        public bool IsDeleted { get; set; }
    }
}

