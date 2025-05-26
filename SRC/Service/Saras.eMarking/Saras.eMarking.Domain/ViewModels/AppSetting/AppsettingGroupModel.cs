using Saras.eMarking.Domain.Extensions;

namespace Saras.eMarking.Domain.ViewModels
{
    public class AppsettingGroupModel
    {
        public AppsettingGroupModel()
        {
        }

        public int SettingGroupID { get; set; }
        [XssTextValidation]
        public string SettingGroupCode { get; set; }
        [XssTextValidation]
        public string SettingGroupName { get; set; }
        [XssTextValidation]
        public string Description { get; set; }
        public long? OrganizationID { get; set; }
        public bool IsDeleted { get; set; }
    }
}