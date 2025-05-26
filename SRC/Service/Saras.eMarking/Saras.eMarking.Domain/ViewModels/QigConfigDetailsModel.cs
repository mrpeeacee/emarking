using Saras.eMarking.Domain.Extensions;

namespace Saras.eMarking.Domain.ViewModels
{
    public class QigConfigDetailsModel
    {
        public QigConfigDetailsModel()
        {

        }

        public long? EntityID { get; set; }
        [XssTextValidation]
        public bool? Value { get; set; }
        [XssTextValidation]
        public string DefaultValue { get; set; }
        public byte ValueType { get; set; }
        public long? AppsettingGroupID { get; set; }
        [XssTextValidation]
        public string AppsettingKey { get; set; }
        [XssTextValidation]
        public string AppsettingKeyName { get; set; }
        [XssTextValidation]
        public string SettingGroupCode { get; set; }
        [XssTextValidation]
        public string SettingGroupName { get; set; }


    }
}
