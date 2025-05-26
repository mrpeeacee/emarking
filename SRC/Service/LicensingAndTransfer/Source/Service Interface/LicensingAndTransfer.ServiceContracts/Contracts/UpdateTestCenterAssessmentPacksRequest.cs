using System.ServiceModel;
using System.Collections.Generic;

namespace LicensingAndTransfer.ServiceContracts
{
    [MessageContract]
    public class UpdateTestCenterAssessmentPacksRequest
    {
        [MessageBodyMember(Order = 0)]
        private List<DataContracts.TestCenterAssessmentPacks> ListTestCenterAssessmentPacksField;
        public List<DataContracts.TestCenterAssessmentPacks> ListTestCenterAssessmentPacks
        {
            get
            {
                return this.ListTestCenterAssessmentPacksField;
            }
            set
            {
                this.ListTestCenterAssessmentPacksField = value;
            }
        }        
    }
}
